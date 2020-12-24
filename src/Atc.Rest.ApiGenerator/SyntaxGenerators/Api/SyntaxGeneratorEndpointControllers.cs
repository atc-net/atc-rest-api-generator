using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public class SyntaxGeneratorEndpointControllers : ISyntaxGeneratorEndpointControllers
    {
        public SyntaxGeneratorEndpointControllers(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            string focusOnSegmentName)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        }

        private ApiProjectOptions ApiProjectOptions { get; }

        private List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public bool GenerateCode()
        {
            var controllerTypeName = FocusOnSegmentName.EnsureFirstCharacterToUpper() + "Controller";

            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(
                ApiProjectOptions,
                NameConstants.Endpoints);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.Create(controllerTypeName);
            if (ApiProjectOptions.ApiOptions.Generator.UseAuthorization)
            {
                classDeclaration =
                    classDeclaration.AddAttributeLists(
                        SyntaxAttributeListFactory.Create(nameof(AuthorizeAttribute)));
            }

            classDeclaration =
                classDeclaration.AddAttributeLists(
                    SyntaxAttributeListFactory.Create(nameof(ApiControllerAttribute)),
                    SyntaxAttributeListFactory.CreateWithOneItemWithOneArgument(nameof(RouteAttribute), $"api/{ApiProjectOptions.ApiVersion}/{GetRouteSegment()}"))
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(nameof(ControllerBase))))
                .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForEndpoints(FocusOnSegmentName));

            // Create Methods
            var usedApiOperations = new List<OpenApiOperation>();
            foreach (var (key, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
            {
                var hasRouteParameters = value.HasParameters();
                foreach (var apiOperation in value.Operations)
                {
                    var methodDeclaration = CreateMembersForEndpoints(apiOperation, key, FocusOnSegmentName, hasRouteParameters)
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForEndpointMethods(apiOperation, FocusOnSegmentName));
                    classDeclaration = classDeclaration.AddMembers(methodDeclaration);

                    usedApiOperations.Add(apiOperation.Value);
                }
            }

            // Create private part for methods
            foreach (var (_, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
            {
                var hasRouteParameters = value.HasParameters();
                foreach (var apiOperation in value.Operations)
                {
                    var methodDeclaration = CreateMembersForEndpointsPrivateHelper(apiOperation, hasRouteParameters);
                    classDeclaration = classDeclaration.AddMembers(methodDeclaration);

                    usedApiOperations.Add(apiOperation.Value);
                }
            }

            // Add the class to the namespace.
            @namespace = @namespace.AddMembers(classDeclaration);

            // Add using statement to compilationUnit
            var includeRestResults = classDeclaration
                .Select<IdentifierNameSyntax>()
                .Any(x => x.Identifier.ValueText.Contains($"({Microsoft.OpenApi.Models.NameConstants.Pagination}<", StringComparison.Ordinal));

            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectEndpointsFactory.CreateUsingList(
                    ApiProjectOptions,
                    FocusOnSegmentName,
                    usedApiOperations,
                    includeRestResults,
                    HasSharedResponseContract()));

            // Add namespace to compilationUnit
            compilationUnit = compilationUnit.AddMembers(@namespace);

            // Set code property
            Code = compilationUnit;
            return true;
        }

        public string ToCodeAsString()
        {
            if (Code == null)
            {
                GenerateCode();
            }

            if (Code == null)
            {
                return $"Syntax generate problem for endpoints-controller for: {FocusOnSegmentName}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString();
        }

        public LogKeyValueItem ToFile()
        {
            var controllerName = FocusOnSegmentName.EnsureFirstCharacterToUpper() + "Controller";
            var file = Util.GetCsFileNameForEndpoints(ApiProjectOptions.PathForEndpoints, controllerName);
            return TextFileHelper.Save(file, ToCodeAsString());
        }

        public void ToFile(FileInfo file)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            TextFileHelper.Save(file, ToCodeAsString());
        }

        public List<EndpointMethodMetadata> GetMetadataForMethods()
        {
            var list = new List<EndpointMethodMetadata>();
            var hasSharedResponseContract = HasSharedResponseContract();
            foreach (var (key, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
            {
                var generatorParameters = new SyntaxGeneratorContractParameters(ApiProjectOptions, FocusOnSegmentName);
                var generatedParameters = generatorParameters.GenerateSyntaxTrees();

                foreach (var apiOperation in value.Operations)
                {
                    var httpAttributeRoutePart = GetHttpAttributeRoutePart(key);
                    var routePart = string.IsNullOrEmpty(httpAttributeRoutePart)
                        ? $"/{GetRouteSegment()}"
                        : $"/{GetRouteSegment()}/{httpAttributeRoutePart}";
                    var operationName = apiOperation.Value.GetOperationName();

                    string? contractParameterTypeName = null;
                    if (apiOperation.Value.HasParametersOrRequestBody() || value.HasParameters())
                    {
                        contractParameterTypeName = operationName + NameConstants.ContractParameters;
                    }

                    var sgContractParameter = generatedParameters.FirstOrDefault(x => x.ApiOperation.GetOperationName() == operationName);

                    var responseTypeNames = GetResponseTypeNames(apiOperation.Value.Responses, FocusOnSegmentName, operationName);

                    if (contractParameterTypeName != null &&
                        responseTypeNames.FirstOrDefault(x => x.Item1 == HttpStatusCode.BadRequest) == null)
                    {
                        responseTypeNames.Add(
                            new Tuple<HttpStatusCode, string>(
                                HttpStatusCode.BadRequest,
                                "Validation"));
                    }

                    var responseTypeNamesAndItemSchema = GetResponseTypeNamesAndItemSchema(responseTypeNames);

                    var endpointMethodMetadata = new EndpointMethodMetadata(
                        ApiProjectOptions.ProjectName,
                        FocusOnSegmentName,
                        $"/api/{ApiProjectOptions.ApiVersion}{routePart}",
                        apiOperation.Key,
                        operationName,
                        hasSharedResponseContract,
                        "I" + operationName + NameConstants.ContractHandler,
                        contractParameterTypeName,
                        operationName + NameConstants.ContractResult,
                        responseTypeNamesAndItemSchema,
                        sgContractParameter,
                        ApiProjectOptions.Document.Components.Schemas);

                    list.Add(endpointMethodMetadata);
                }
            }

            return list;
        }

        private string GetRouteSegment()
        {
            var (key, _) = ApiProjectOptions.Document.Paths
                .FirstOrDefault(x => x.IsPathStartingSegmentName(FocusOnSegmentName));

            return key?
                       .Split('/', StringSplitOptions.RemoveEmptyEntries)?
                       .FirstOrDefault()
                   ?? throw new NotSupportedException("SegmentName was not found in any route.");
        }

        private bool HasSharedResponseContract()
        {
            foreach (var (_, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(FocusOnSegmentName))
            {
                foreach (var apiOperation in value.Operations)
                {
                    if (apiOperation.Value.Responses == null)
                    {
                        continue;
                    }

                    var responseModelName = apiOperation.Value.Responses.GetModelNameForStatusCode(HttpStatusCode.OK);
                    var isSharedResponseModel = !string.IsNullOrEmpty(responseModelName) && OperationSchemaMappings.IsShared(responseModelName);
                    if (isSharedResponseModel)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private List<Tuple<HttpStatusCode, string, OpenApiSchema?>> GetResponseTypeNamesAndItemSchema(List<Tuple<HttpStatusCode, string>> responseTypeNames)
        {
            var list = new List<Tuple<HttpStatusCode, string, OpenApiSchema?>>();
            foreach (var responseTypeName in responseTypeNames)
            {
                if (string.IsNullOrEmpty(responseTypeName.Item2))
                {
                    list.Add(new Tuple<HttpStatusCode, string, OpenApiSchema?>(responseTypeName.Item1, responseTypeName.Item2, null!));
                }
                else
                {
                    var rawModelName = OpenApiDocumentSchemaModelNameHelper.GetRawModelName(responseTypeName.Item2);

                    var fullModelName = OpenApiDocumentSchemaModelNameHelper.EnsureModelNameWithNamespaceIfNeeded(
                        ApiProjectOptions.ProjectName,
                        FocusOnSegmentName,
                        responseTypeName.Item2);

                    var schema = ApiProjectOptions.Document.Components.Schemas.FirstOrDefault(x => x.Key.Equals(rawModelName, StringComparison.OrdinalIgnoreCase));
                    list.Add(new Tuple<HttpStatusCode, string, OpenApiSchema?>(responseTypeName.Item1, fullModelName, schema.Value));
                }
            }

            return list;
        }

        private List<Tuple<HttpStatusCode, string>> GetResponseTypeNames(OpenApiResponses openApiResponses, string segmentName, string operationName)
        {
            var list = new List<Tuple<HttpStatusCode, string>>();

            var httpStatusCodes = openApiResponses.GetHttpStatusCodes();
            var producesResponseAttributeParts = openApiResponses.GetProducesResponseAttributeParts(
                operationName + NameConstants.ContractResult,
                false,
                segmentName,
                OperationSchemaMappings,
                ApiProjectOptions.ProjectName);

            foreach (var producesResponseAttributePart in producesResponseAttributeParts)
            {
                var s = producesResponseAttributePart
                    .Replace("ProducesResponseType(", string.Empty, StringComparison.Ordinal)
                    .Replace("typeof(", string.Empty, StringComparison.Ordinal)
                    .Replace("StatusCodes.Status", string.Empty, StringComparison.Ordinal)
                    .Replace(")", string.Empty, StringComparison.Ordinal)
                    .Replace(" ", string.Empty, StringComparison.Ordinal);
                var sa = s.Split(',');

                switch (sa.Length)
                {
                    case 1:
                        {
                            foreach (var httpStatusCode in httpStatusCodes)
                            {
                                if (sa[0].IndexOf(((int)httpStatusCode).ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.Ordinal) != -1)
                                {
                                    list.Add(
                                        new Tuple<HttpStatusCode, string>(
                                            httpStatusCode,
                                            string.Empty));
                                }
                            }

                            break;
                        }

                    case 2:
                        {
                            foreach (var httpStatusCode in httpStatusCodes)
                            {
                                if (sa[1].IndexOf(((int)httpStatusCode).ToString(GlobalizationConstants.EnglishCultureInfo), StringComparison.Ordinal) != -1)
                                {
                                    list.Add(
                                        new Tuple<HttpStatusCode, string>(
                                            httpStatusCode,
                                            sa[0]));
                                }
                            }

                            break;
                        }
                }
            }

            return list;
        }

        private MethodDeclarationSyntax CreateMembersForEndpoints(
            KeyValuePair<OperationType, OpenApiOperation> apiOperation,
            string urlPath,
            string area,
            bool hasRouteParameters)
        {
            var operationName = apiOperation.Value.GetOperationName();
            var interfaceName = "I" + operationName + NameConstants.ContractHandler;
            var methodName = operationName + "Async";
            var helperMethodName = $"Invoke{operationName}Async";
            var parameterTypeName = operationName + NameConstants.ContractParameters;
            var resultTypeName = operationName + NameConstants.ContractResult;

            // Create method # use CreateParameterList & CreateCodeBlockReturnStatement
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(nameof(ActionResult))),
                    SyntaxFactory.Identifier(methodName))
                .AddModifiers(SyntaxTokenFactory.PublicKeyword())
                .WithParameterList(CreateParameterList(apiOperation.Value.HasParametersOrRequestBody() || hasRouteParameters, parameterTypeName, interfaceName, true))
                .WithBody(
                    SyntaxFactory.Block(
                        SyntaxIfStatementFactory.CreateParameterArgumentNullCheck("handler", false),
                        CreateCodeBlockReturnStatement(helperMethodName, apiOperation.Value.HasParametersOrRequestBody() || hasRouteParameters)));

            // Create and add Http-method-attribute
            var httpAttributeRoutePart = GetHttpAttributeRoutePart(urlPath);
            methodDeclaration = string.IsNullOrEmpty(httpAttributeRoutePart)
                ? methodDeclaration.AddAttributeLists(
                    SyntaxAttributeListFactory.Create($"Http{apiOperation.Key}"))
                : methodDeclaration.AddAttributeLists(
                    SyntaxAttributeListFactory.CreateWithOneItemWithOneArgument(
                        $"Http{apiOperation.Key}",
                        httpAttributeRoutePart));

            // Create and add producesResponseTypes-attributes
            var producesResponseAttributeParts = apiOperation.Value.Responses.GetProducesResponseAttributeParts(
                resultTypeName,
                ApiProjectOptions.ApiOptions.Generator.Response.UseProblemDetailsAsDefaultBody,
                area,
                OperationSchemaMappings,
                ApiProjectOptions.ProjectName);

            return producesResponseAttributeParts
                .Aggregate(
                    methodDeclaration,
                    (current, producesResponseAttributePart) => current.AddAttributeLists(
                        SyntaxAttributeListFactory.Create(producesResponseAttributePart)));
        }

        private static MethodDeclarationSyntax CreateMembersForEndpointsPrivateHelper(
            KeyValuePair<OperationType, OpenApiOperation> apiOperation,
            bool hasRouteParameters)
        {
            var operationName = apiOperation.Value.GetOperationName();
            var interfaceName = "I" + operationName + NameConstants.ContractHandler;
            var methodName = $"Invoke{operationName}Async";
            var parameterTypeName = operationName + NameConstants.ContractParameters;
            var hasParametersOrRequestBody = apiOperation.Value.HasParametersOrRequestBody() || hasRouteParameters;

            // Create method # use CreateParameterList & CreateCodeBlockReturnStatement
            var methodDeclaration = SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(nameof(ActionResult))),
                    SyntaxFactory.Identifier(methodName))
                .AddModifiers(SyntaxTokenFactory.PrivateKeyword())
                .AddModifiers(SyntaxTokenFactory.StaticKeyword())
                .AddModifiers(SyntaxTokenFactory.AsyncKeyword())
                .WithParameterList(CreateParameterList(hasParametersOrRequestBody, parameterTypeName, interfaceName, false))
                .WithBody(
                    SyntaxFactory.Block(
                        CreateCodeBlockReturnStatementForHelper(hasParametersOrRequestBody)));

            return methodDeclaration;
        }

        private static string GetHttpAttributeRoutePart(string urlPath)
        {
            var urlPathParts = urlPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (urlPathParts.Length <= 1)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (var i = 1; i < urlPathParts.Length; i++)
            {
                if (i != 1)
                {
                    sb.Append('/');
                }

                sb.Append(urlPathParts[i]);
            }

            return sb.ToString();
        }

        private static ParameterListSyntax CreateParameterList(
            bool hasParametersOrRequestBody,
            string parameterTypeName,
            string interfaceName,
            bool useFromServicesAttributeOnInterface)
        {
            ParameterListSyntax parameterList;
            if (hasParametersOrRequestBody)
            {
                if (useFromServicesAttributeOnInterface)
                {
                    parameterList = SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                                SyntaxTokenFactory.Comma(),
                                SyntaxParameterFactory.CreateWithAttribute(nameof(FromServicesAttribute), interfaceName, "handler"),
                                SyntaxTokenFactory.Comma(),
                                SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                            }));
                }
                else
                {
                    parameterList = SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList<ParameterSyntax>(
                            new SyntaxNodeOrToken[]
                            {
                                SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                                SyntaxTokenFactory.Comma(),
                                SyntaxParameterFactory.Create(interfaceName, "handler"),
                                SyntaxTokenFactory.Comma(),
                                SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                            }));
                }
            }
            else
            {
                parameterList = SyntaxFactory.ParameterList(
                    SyntaxFactory.SeparatedList<ParameterSyntax>(
                        new SyntaxNodeOrToken[]
                        {
                            SyntaxParameterFactory.CreateWithAttribute(nameof(FromServicesAttribute), interfaceName, "handler"),
                            SyntaxTokenFactory.Comma(),
                            SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower()),
                        }));
            }

            return parameterList;
        }

        private static ReturnStatementSyntax CreateCodeBlockReturnStatement(string helperMethodName, bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxArgumentFactory.Create("parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxArgumentFactory.Create("handler"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxArgumentFactory.Create("cancellationToken"),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxArgumentFactory.Create("handler"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxArgumentFactory.Create("cancellationToken"),
                };

            return SyntaxFactory.ReturnStatement(
                SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(helperMethodName))
                    .WithArgumentList(
                        SyntaxFactory.ArgumentList(
                            SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments))));
        }

        private static ReturnStatementSyntax CreateCodeBlockReturnStatementForHelper(bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxArgumentFactory.Create("parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxArgumentFactory.Create("cancellationToken"),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxArgumentFactory.Create("cancellationToken"),
                };

            return SyntaxFactory.ReturnStatement(
                SyntaxFactory.AwaitExpression(
                    SyntaxFactory.InvocationExpression(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName("handler"),
                                SyntaxFactory.IdentifierName("ExecuteAsync")))
                        .WithArgumentList(
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList<ArgumentSyntax>(arguments)))));
        }
    }
}
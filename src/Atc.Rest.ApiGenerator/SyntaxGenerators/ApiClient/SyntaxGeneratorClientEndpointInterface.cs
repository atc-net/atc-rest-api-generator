using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient
{
    public class SyntaxGeneratorClientEndpointInterface
    {
        public SyntaxGeneratorClientEndpointInterface(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName,
            string urlPath,
            bool hasParametersOrRequestBody)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.ApiOperationType = apiOperationType;
            this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
            this.ApiUrlPath = urlPath ?? throw new ArgumentNullException(nameof(urlPath));
            this.HasParametersOrRequestBody = hasParametersOrRequestBody;
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        private List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public OperationType ApiOperationType { get; }

        public OpenApiOperation ApiOperation { get; }

        public string ApiUrlPath { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.Endpoint;

        public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        public bool HasParametersOrRequestBody { get; }

        public bool GenerateCode()
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(
                ApiProjectOptions,
                NameConstants.Endpoints,
                FocusOnSegmentName);

            // Create interface
            var interfaceDeclaration = SyntaxInterfaceDeclarationFactory.Create(InterfaceTypeName)
                .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

            // Create interface-method
            interfaceDeclaration = interfaceDeclaration.AddMembers(CreateMembers());
            //// TODO: var methodDeclaration = ...

            // Add using statement to compilationUnit
            var includeRestResults = interfaceDeclaration
                .Select<IdentifierNameSyntax>()
                .Any(x => x.Identifier.ValueText.Contains(
                    $"({Microsoft.OpenApi.Models.NameConstants.Pagination}<",
                    StringComparison.Ordinal));
            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectApiClientFactory.CreateUsingListForEndpointInterface(
                    ApiProjectOptions,
                    includeRestResults,
                    ContractHelper.HasSharedResponseContract(
                        ApiProjectOptions.Document,
                        OperationSchemaMappings,
                        FocusOnSegmentName)));

            // Add interface-method to interface
            //// TODO: interfaceDeclaration = interfaceDeclaration.AddMembers(methodDeclaration);

            // Add the interface to the namespace.
            @namespace = @namespace.AddMembers(interfaceDeclaration);

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
                return $"Syntax generate problem for client-endpoint-interface for apiOperation: {ApiOperation}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString();
        }

        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var file = Util.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, NameConstants.EndpointInterfaces, InterfaceTypeName);
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

        public override string ToString()
        {
            return $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
        }

        private MemberDeclarationSyntax[] CreateMembers()
        {
            var responseTypes = ApiOperation.Responses.GetResponseTypes(
                OperationSchemaMappings,
                FocusOnSegmentName,
                ApiProjectOptions.ProjectName,
                ensureModelNameWithNamespaceIfNeeded: false,
                useProblemDetailsAsDefaultResponseBody: false,
                false);

            string resultTypeName = responseTypes
                .FirstOrDefault(x => x.Item1 == HttpStatusCode.OK)?.Item2 ?? responseTypes
                .FirstOrDefault(x => x.Item1 == HttpStatusCode.Created)?.Item2 ?? "string";

            var result = new List<MemberDeclarationSyntax>
            {
                CreateExecuteAsyncMethod(ParameterTypeName, resultTypeName, HasParametersOrRequestBody),
            };

            return result.ToArray();
        }

        private MemberDeclarationSyntax CreateExecuteAsyncMethod(string parameterTypeName, string resultTypeName, bool hasParameters)
        {
            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                };

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                    SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier("EndpointResult"))
                                        .WithTypeArgumentList(
                                            SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName))))),
                    SyntaxFactory.Identifier("ExecuteAsync"))
                .WithModifiers(SyntaxTokenListFactory.PublicKeyword())
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }
    }
}

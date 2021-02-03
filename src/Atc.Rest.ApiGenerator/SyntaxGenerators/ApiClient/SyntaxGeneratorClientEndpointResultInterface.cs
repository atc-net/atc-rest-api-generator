using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Atc.Rest.Client;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient
{
    public class SyntaxGeneratorClientEndpointResultInterface : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
    {
        public SyntaxGeneratorClientEndpointResultInterface(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            IList<OpenApiParameter> globalPathParameters,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName,
            string urlPath,
            bool hasParametersOrRequestBody)
            : base(
                apiProjectOptions,
                operationSchemaMappings,
                globalPathParameters,
                apiOperationType,
                apiOperation,
                focusOnSegmentName,
                urlPath,
                hasParametersOrRequestBody)
        {
        }

        public CompilationUnitSyntax? Code { get; private set; }

        public string InterfaceTypeName => "I" + ApiOperation.GetOperationName() + NameConstants.EndpointResult;

        public string ParameterTypeName => ApiOperation.GetOperationName() + NameConstants.ContractParameters;

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
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(nameof(IEndpointResponse))))
                .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForResults(ApiOperation, FocusOnSegmentName));

            interfaceDeclaration = interfaceDeclaration.AddMembers(CreatePropertiesForIsStatusCode());
            interfaceDeclaration = interfaceDeclaration.AddMembers(CreatePropertiesForStatusCodeContent());

            // Add using statement to compilationUnit
            var includeRestResults = interfaceDeclaration
                .Select<IdentifierNameSyntax>()
                .Any(x => x.Identifier.ValueText.Contains(
                    $"({Microsoft.OpenApi.Models.NameConstants.Pagination}<",
                    StringComparison.Ordinal));

            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectApiClientFactory.CreateUsingListForEndpointResultInterface(
                    ApiProjectOptions,
                    includeRestResults,
                    ContractHelper.HasList(ResultTypeName),
                    ContractHelper.HasSharedResponseContract(
                        ApiProjectOptions.Document,
                        OperationSchemaMappings,
                        FocusOnSegmentName)));

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
                return $"Syntax generate problem for client-endpointResult-interface for apiOperation: {ApiOperation}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString()
                .FormatAutoPropertiesOnOneLine();
        }

        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var file = Util.GetCsFileNameForContract(ApiProjectOptions.PathForEndpoints, area, NameConstants.EndpointInterfaceResults, InterfaceTypeName);
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
            return
                $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
        }

        private MemberDeclarationSyntax[] CreatePropertiesForIsStatusCode()
        {
            return ResponseTypes
                .Select(x => CreatePropertyForIsStatusCode(x.Item1))
                .ToArray();
        }

        private MemberDeclarationSyntax CreatePropertyForIsStatusCode(HttpStatusCode statusCode)
        {
            return SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.PredefinedType(
                        SyntaxFactory.Token(SyntaxKind.BoolKeyword)),
                    SyntaxFactory.Identifier("Is" + statusCode.ToNormalizedString()))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.SingletonList(
                            SyntaxFactory.AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration)
                            .WithSemicolonToken(
                                SyntaxTokenFactory.Semicolon()))));
        }

        private MemberDeclarationSyntax[] CreatePropertiesForStatusCodeContent()
        {
            var responseTypes = ApiOperation.Responses.GetResponseTypes(
                OperationSchemaMappings,
                FocusOnSegmentName,
                ApiProjectOptions.ProjectName,
                ensureModelNameWithNamespaceIfNeeded: false,
                useProblemDetailsAsDefaultResponseBody: true,
                includeEmptyResponseTypes: false,
                HasParametersOrRequestBody,
                ApiProjectOptions.ApiOptions.Generator.UseAuthorization,
                includeIfNotDefinedInternalServerError: true);

            return responseTypes
                .Select(x => CreatePropertyForStatusCodeContent(x.Item1, x.Item2))
                .ToArray();
        }

        private MemberDeclarationSyntax CreatePropertyForStatusCodeContent(HttpStatusCode statusCode, string resultTypeName)
        {
            return SyntaxFactory.PropertyDeclaration(
                SyntaxFactory.IdentifierName(resultTypeName),
                SyntaxFactory.Identifier(statusCode.ToNormalizedString() + "Content"))
                .WithAccessorList(
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.SingletonList(
                            SyntaxFactory.AccessorDeclaration(
                                    SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(
                                    SyntaxTokenFactory.Semicolon()))));
        }
    }
}
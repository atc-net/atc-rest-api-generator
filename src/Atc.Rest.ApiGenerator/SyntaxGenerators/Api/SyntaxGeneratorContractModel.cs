using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Atc.Rest.ApiGenerator.SyntaxFactories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public class SyntaxGeneratorContractModel : ISyntaxSchemaCodeGenerator
    {
        public SyntaxGeneratorContractModel(
            ApiProjectOptions apiProjectOptions,
            string apiSchemaKey,
            OpenApiSchema apiSchema,
            string focusOnSegmentName)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.ApiSchemaKey = apiSchemaKey ?? throw new ArgumentNullException(nameof(apiSchemaKey));
            this.ApiSchema = apiSchema ?? throw new ArgumentNullException(nameof(apiSchema));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
            if (this.FocusOnSegmentName == "#")
            {
                this.IsSharedContract = true;
            }
        }

        private ApiProjectOptions ApiProjectOptions { get; }

        private bool IsSharedContract { get; set; }

        public string ApiSchemaKey { get; }

        public OpenApiSchema ApiSchema { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public bool IsEnum { get; private set; }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        public bool GenerateCode()
        {
            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            NamespaceDeclarationSyntax @namespace;
            if (ApiSchema.IsSchemaEnumOrPropertyEnum())
            {
                IsEnum = true;

                // Create a namespace
                @namespace = SyntaxProjectFactory.CreateNamespace(
                    ApiProjectOptions,
                    NameConstants.Contracts);

                var apiEnumSchema = ApiSchema.GetEnumSchema();

                // Create an enum
                var enumDeclaration = SyntaxEnumFactory.Create(apiEnumSchema.Item1.EnsureFirstCharacterToUpper(), apiEnumSchema.Item2);

                if (enumDeclaration.HasAttributeOfAttributeType(typeof(FlagsAttribute)))
                {
                    // Add using statement to compilationUnit
                    compilationUnit = compilationUnit.AddUsingStatements(new[] { "System" });
                }

                if (enumDeclaration.HasAttributeOfAttributeType(typeof(SuppressMessageAttribute)))
                {
                    // Add using statement to compilationUnit
                    compilationUnit = compilationUnit.AddUsingStatements(new[] { "System.Diagnostics.CodeAnalysis" });
                }

                // Add the enum to the namespace.
                @namespace = @namespace.AddMembers(enumDeclaration);
            }
            else
            {
                // Create a namespace
                @namespace = IsSharedContract
                    ? SyntaxProjectFactory.CreateNamespace(ApiProjectOptions, NameConstants.Contracts)
                    : SyntaxProjectFactory.CreateNamespace(ApiProjectOptions, NameConstants.Contracts, FocusOnSegmentName);

                // Create class
                var classDeclaration = SyntaxClassDeclarationFactory.Create(ApiSchemaKey.EnsureFirstCharacterToUpper())
                    .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                    .WithLeadingTrivia(SyntaxDocumentationFactory.Create(ApiSchema));

                // Create class-properties and add to class
                if (ApiSchema.Properties != null)
                {
                    if (ApiSchema.Type == OpenApiDataTypeConstants.Array)
                    {
                        var (key, _) = ApiProjectOptions.Document.Components.Schemas.FirstOrDefault(x => x.Key.Equals(ApiSchema.Title, StringComparison.OrdinalIgnoreCase));
                        if (string.IsNullOrEmpty(ApiSchema.Title))
                        {
                            ApiSchema.Title = ApiSchemaKey;
                            key = ApiSchemaKey;
                        }

                        if (string.IsNullOrEmpty(ApiSchema.Items.Title))
                        {
                            ApiSchema.Items.Title = ApiSchemaKey;
                        }

                        var title = key != null
                            ? $"{ApiSchema.Title.EnsureFirstCharacterToUpperAndSingular()}List"
                            : ApiSchema.Title.EnsureFirstCharacterToUpper();

                        var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateListAuto(ApiSchema.Items.Title, title)
                            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateSummary($"A list of {ApiSchema.Items.Title}."));
                        classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
                    }
                    else
                    {
                        foreach (var property in ApiSchema.Properties)
                        {
                            var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                                    property,
                                    ApiSchema.Required,
                                    ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes)
                                .WithLeadingTrivia(SyntaxDocumentationFactory.Create(property.Value));
                            classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
                        }
                    }

                    var methodDeclaration = SyntaxMethodDeclarationFactory.CreateToStringMethod(ApiSchema.Properties);
                    if (methodDeclaration != null)
                    {
                        methodDeclaration = methodDeclaration.WithLeadingTrivia(SyntaxDocumentationFactory.CreateForOverrideToString());
                        classDeclaration = classDeclaration.AddMembers(methodDeclaration);
                    }
                }

                // Add using statement to compilationUnit
                compilationUnit = compilationUnit.AddUsingStatements(ProjectContractDataFactory.CreateUsingList(ApiSchema));

                // Add the class to the namespace.
                @namespace = @namespace.AddMembers(classDeclaration);
            }

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
                return $"Syntax generate problem for contract-model for schema: {ApiSchemaKey}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString()
                .FormatAutoPropertiesOnOneLine()
                .FormatPublicPrivateLines()
                .FormatDoubleLines();
        }

        [SuppressMessage("Major Code Smell", "S3358:Ternary operators should not be nested", Justification = "OK.")]
        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var modelName = ApiSchemaKey.EnsureFirstCharacterToUpper();
            var file = IsEnum
                ? Util.GetCsFileNameForContractEnumTypes(ApiProjectOptions.PathForContracts, modelName)
                : IsSharedContract
                    ? Util.GetCsFileNameForContractShared(ApiProjectOptions.PathForContractsShared, modelName)
                    : Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractModels, modelName);
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
            return $"{nameof(ApiSchemaKey)}: {ApiSchemaKey}, SegmentName: {FocusOnSegmentName}, IsShared: {IsSharedContract}, {nameof(IsEnum)}: {IsEnum}";
        }
    }
}
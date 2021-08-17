using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Atc.CodeAnalysis.CSharp;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Factories;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.ProjectSyntaxFactories;
using Atc.Rest.ApiGenerator.SyntaxFactories;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api.Interfaces;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public class SyntaxGeneratorContractParameter : ISyntaxOperationCodeGenerator
    {
        public SyntaxGeneratorContractParameter(
            ApiProjectOptions apiProjectOptions,
            IList<OpenApiParameter> globalPathParameters,
            OperationType apiOperationType,
            OpenApiOperation apiOperation,
            string focusOnSegmentName)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
            this.ApiOperationType = apiOperationType;
            this.ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));

            this.IsForClient = false;
            this.UseOwnFolder = true;
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        public IList<OpenApiParameter> GlobalPathParameters { get; }

        public OperationType ApiOperationType { get; }

        public OpenApiOperation ApiOperation { get; }

        public string FocusOnSegmentName { get; }

        public CompilationUnitSyntax? Code { get; private set; }

        public bool IsForClient { get; set; }

        public bool UseOwnFolder { get; set; }

        public bool GenerateCode()
        {
            var parameterTypeName = ApiOperation.GetOperationName() + NameConstants.ContractParameters;

            // Create compilationUnit
            var compilationUnit = SyntaxFactory.CompilationUnit();

            // Create a namespace
            var @namespace = SyntaxProjectFactory.CreateNamespace(
                ApiProjectOptions,
                NameConstants.Contracts,
                FocusOnSegmentName);

            // Create class
            var classDeclaration = SyntaxClassDeclarationFactory.Create(parameterTypeName)
                .AddGeneratedCodeAttribute(ApiProjectOptions.ToolName, ApiProjectOptions.ToolVersion.ToString())
                .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameters(ApiOperation, FocusOnSegmentName));

            // Add properties to the class
            if (GlobalPathParameters.Any())
            {
                foreach (var parameter in GlobalPathParameters)
                {
                    var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                            parameter,
                            ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                            ApiProjectOptions.IsForClient)
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(parameter));
                    classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
                }
            }

            if (ApiOperation.Parameters != null)
            {
                foreach (var parameter in ApiOperation.Parameters)
                {
                    var propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                            parameter,
                            ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                            ApiProjectOptions.IsForClient)
                        .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(parameter));
                    classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
                }
            }

            var requestSchema = ApiOperation.RequestBody?.Content?.GetSchemaByFirstMediaType();

            if (ApiOperation.RequestBody != null && requestSchema != null)
            {
                var isFormatTypeOfBinary = requestSchema.IsFormatTypeOfBinary();
                var isItemsOfFormatTypeBinary = requestSchema.IsItemsOfFormatTypeBinary();

                var requestBodyType = "string?";
                if (requestSchema.Reference is not null)
                {
                    requestBodyType = requestSchema.Reference.Id.EnsureFirstCharacterToUpper();
                }
                else if (isFormatTypeOfBinary)
                {
                    requestBodyType = "IFormFile";
                }
                else if (isItemsOfFormatTypeBinary)
                {
                    requestBodyType = "IFormFile";
                }
                else if (requestSchema.Items is not null)
                {
                    requestBodyType = requestSchema.Items.Reference.Id.EnsureFirstCharacterToUpper();
                }

                PropertyDeclarationSyntax propertyDeclaration;
                if (requestSchema.Type == OpenApiDataTypeConstants.Array)
                {
                    if (ApiProjectOptions.IsForClient)
                    {
                        propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateListAuto(requestBodyType, NameConstants.Request)
                            .AddValidationAttribute(new RequiredAttribute())
                            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                    }
                    else
                    {
                        propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateListAuto(requestBodyType, NameConstants.Request);

                        propertyDeclaration = requestSchema.IsItemsOfFormatTypeBinary()
                            ? propertyDeclaration.AddFromFormAttribute()
                            : propertyDeclaration.AddFromBodyAttribute();

                        propertyDeclaration = propertyDeclaration
                            .AddValidationAttribute(new RequiredAttribute())
                            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                    }
                }
                else
                {
                    if (ApiProjectOptions.IsForClient)
                    {
                        propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                                parameterLocation: null,
                                isNullable: false,
                                isRequired: true,
                                requestBodyType,
                                NameConstants.Request,
                                ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                                initializer: null)
                            .AddValidationAttribute(new RequiredAttribute())
                            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                    }
                    else
                    {
                        propertyDeclaration = SyntaxPropertyDeclarationFactory.CreateAuto(
                                parameterLocation: null,
                                isNullable: false,
                                isRequired: true,
                                requestBodyType,
                                NameConstants.Request,
                                ApiProjectOptions.ApiOptions.Generator.UseNullableReferenceTypes,
                                initializer: null);

                        propertyDeclaration = requestSchema.HasAnyPropertiesFormatTypeBinary() || requestSchema.HasAnyPropertiesOfArrayWithFormatTypeBinary()
                            ? propertyDeclaration.AddFromFormAttribute()
                            : propertyDeclaration.AddFromBodyAttribute();

                        propertyDeclaration = propertyDeclaration
                            .AddValidationAttribute(new RequiredAttribute())
                            .WithLeadingTrivia(SyntaxDocumentationFactory.CreateForParameter(ApiOperation.RequestBody));
                    }
                }

                classDeclaration = classDeclaration.AddMembers(propertyDeclaration);
            }

            var methodDeclaration = SyntaxMethodDeclarationFactory.CreateToStringMethod(GlobalPathParameters, ApiOperation.Parameters, ApiOperation.RequestBody);
            if (methodDeclaration != null)
            {
                methodDeclaration = methodDeclaration.WithLeadingTrivia(SyntaxDocumentationFactory.CreateForOverrideToString());
                classDeclaration = classDeclaration.AddMembers(methodDeclaration);
            }

            // Add using statement to compilationUnit
            compilationUnit = compilationUnit.AddUsingStatements(
                ProjectApiFactory.CreateUsingListForContractParameter(
                    GlobalPathParameters,
                    ApiOperation.Parameters,
                    ApiOperation.RequestBody,
                    ApiProjectOptions.IsForClient));

            // Add the class to the namespace.
            @namespace = @namespace.AddMembers(classDeclaration);

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
                return $"Syntax generate problem for contract-parameter for apiOperation: {ApiOperation}";
            }

            return Code
                .NormalizeWhitespace()
                .ToFullString()
                .EnsureEnvironmentNewLines()
                .FormatAutoPropertiesOnOneLine()
                .FormatPublicPrivateLines()
                .FormatDoubleLines();
        }

        public LogKeyValueItem ToFile()
        {
            var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var parameterName = ApiOperation.GetOperationName() + NameConstants.ContractParameters;

            var file = IsForClient
                ? Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ClientRequestParameters, parameterName)
                : UseOwnFolder
                    ? Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractParameters, parameterName)
                    : Util.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, parameterName);

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
    }
}
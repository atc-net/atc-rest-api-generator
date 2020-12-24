using System;
using System.Collections.Generic;
using System.Reflection;
using Atc.Rest.ApiGenerator.Generators;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Helpers.XunitTest;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;
using Atc.XUnit;
using Xunit;
using Xunit.Abstractions;

namespace Atc.Rest.ApiGenerator.Tests
{
    public class CodeComplianceTests
    {
        // ReSharper disable once NotAccessedField.Local
        private readonly ITestOutputHelper testOutputHelper;
        private readonly Assembly sourceAssembly = typeof(AtcRestApiGeneratorAssemblyTypeInitializer).Assembly;
        private readonly Assembly testAssembly = typeof(CodeComplianceTests).Assembly;

        private readonly List<Type> excludeTypes = new List<Type>
        {
            // TODO: Add UnitTest and remove from this list!!
            typeof(EndpointMethodMetadata),
            typeof(DomainProjectOptions),
            typeof(HostProjectOptions),
            typeof(GenerateHelper),
            typeof(ValueTypeTestPropertiesHelper),
            typeof(SolutionAndProjectHelper),
            typeof(ServerApiGenerator),
            typeof(ServerDomainGenerator),
            typeof(ServerHostGenerator),
            typeof(NugetPackageReferenceHelper),
            typeof(TextFileHelper),
            typeof(HttpClientHelper),
            typeof(OpenApiDocumentHelper),
            typeof(OpenApiDocumentValidationHelper),
            typeof(OpenApiOperationSchemaMapHelper),
            typeof(SyntaxGeneratorContractInterface),
            typeof(SyntaxGeneratorContractInterfaces),
            typeof(SyntaxGeneratorContractModel),
            typeof(SyntaxGeneratorContractModels),
            typeof(SyntaxGeneratorContractParameter),
            typeof(SyntaxGeneratorContractParameters),
            typeof(SyntaxGeneratorContractResult),
            typeof(SyntaxGeneratorContractResults),
            typeof(SyntaxGeneratorEndpointControllers),
            typeof(SyntaxGeneratorHandler),
            typeof(SyntaxGeneratorHandlers),
            typeof(ValidatePathsAndOperationsHelper),
            typeof(Util),
            typeof(GenerateXunitTestHelper),
            typeof(GenerateServerApiXunitTestEndpointHandlerStubHelper),
            typeof(GenerateServerApiXunitTestEndpointTestHelper),
            typeof(GenerateServerDomainXunitTestHelper),
            typeof(ParameterCombinationHelper),
            typeof(OpenApiDocumentSchemaModelNameHelper),
        };

        public CodeComplianceTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void AssertExportedMethodsWithMissingTests_AbstractSyntaxTree()
        {
            // Act & Assert
            CodeComplianceTestHelper.AssertExportedMethodsWithMissingTests(
                DecompilerType.AbstractSyntaxTree,
                sourceAssembly,
                testAssembly,
                excludeTypes);
        }

        [Fact]
        public void AssertExportedMethodsWithMissingTests_MonoReflection()
        {
            // Act & Assert
            CodeComplianceTestHelper.AssertExportedMethodsWithMissingTests(
                DecompilerType.MonoReflection,
                sourceAssembly,
                testAssembly,
                excludeTypes);
        }

        [Fact]
        public void AssertExportedTypesWithMissingComments()
        {
            // Act & Assert
            CodeComplianceDocumentationHelper.AssertExportedTypesWithMissingComments(
                sourceAssembly);
        }

        [Fact]
        public void AssertExportedTypesWithWrongNaming()
        {
            // Act & Assert
            CodeComplianceHelper.AssertExportedTypesWithWrongDefinitions(
                sourceAssembly);
        }
    }
}
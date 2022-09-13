using Atc.Rest.ApiGenerator.Extensions;

namespace Atc.Rest.ApiGenerator.Tests;

public class CodeComplianceTests
{
    // ReSharper disable once NotAccessedField.Local
    private readonly ITestOutputHelper testOutputHelper;
    private readonly Assembly sourceAssembly = typeof(AtcRestApiGeneratorAssemblyTypeInitializer).Assembly;
    private readonly Assembly testAssembly = typeof(CodeComplianceTests).Assembly;

    private readonly List<Type> excludeTypes = new()
    {
        // TODO: Add UnitTest and remove from this list!!
        typeof(AtcApiNugetClientHelper),
        typeof(EndpointMethodMetadata),
        typeof(DomainProjectOptions),
        typeof(HostProjectOptions),
        typeof(GenerateHelper),
        typeof(GenerateAtcCodingRulesHelper),
        typeof(ValueTypeTestPropertiesHelper),
        typeof(SolutionAndProjectHelper),
        typeof(ClientCSharpApiGenerator),
        typeof(ServerApiGenerator),
        typeof(ServerDomainGenerator),
        typeof(ServerHostGenerator),
        typeof(ApiGeneratorHelper),
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
        typeof(SyntaxGeneratorClientEndpoint),
        typeof(SyntaxGeneratorClientEndpoints),
        typeof(SyntaxGeneratorEndpointControllers),
        typeof(SyntaxGeneratorClientEndpointInterface),
        typeof(SyntaxGeneratorClientEndpointInterfaces),
        typeof(SyntaxGeneratorClientEndpointResult),
        typeof(SyntaxGeneratorClientEndpointResults),
        typeof(SyntaxGeneratorClientEndpointResultInterface),
        typeof(SyntaxGeneratorClientEndpointResultInterfaces),
        typeof(SyntaxGeneratorHandler),
        typeof(SyntaxGeneratorHandlers),
        typeof(ValidatePathsAndOperationsHelper),
        typeof(DirectoryInfoHelper),
        typeof(GenerateXunitTestHelper),
        typeof(GenerateServerApiXunitTestEndpointHandlerStubHelper),
        typeof(GenerateServerApiXunitTestEndpointTestHelper),
        typeof(GenerateXunitTestPartsHelper),
        typeof(GenerateServerDomainXunitTestHelper),
        typeof(ParameterCombinationHelper),
        typeof(OpenApiDocumentSchemaModelNameHelper),
        typeof(ProjectApiFactory),
        typeof(ProjectApiClientFactory),
        typeof(ProjectHostFactory),
        typeof(ProjectDomainFactory),
        typeof(SyntaxGeneratorSwaggerDocOptions),
        typeof(ListExtensions),
        typeof(GlobalUsingsHelper),
    };

    public CodeComplianceTests(
        ITestOutputHelper testOutputHelper)
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
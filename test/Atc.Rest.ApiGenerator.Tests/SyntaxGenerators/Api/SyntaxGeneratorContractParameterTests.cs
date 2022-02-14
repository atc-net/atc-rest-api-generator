namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

[UsesVerify]
public class SyntaxGeneratorContractParameterTests : SyntaxCodeGeneratorTestBase
{
    public static IEnumerable<object[]> TestInput { get; } = AllTestInput
        .Where(x => x.TestDirectory.Contains("ContractParameter", StringComparison.Ordinal))
        .Select(x => new object[] { x });

    protected override ISyntaxCodeGenerator CreateGenerator(
        ApiProjectOptions apiProject)
    {
        // Verify spec file supported for unit test
        Assert.Single(apiProject.Document.Paths);
        var (_, openApiPathItem) = apiProject.Document.Paths.First();
        Assert.Single(openApiPathItem.Operations);
        var (operationType, openApiOperation) = openApiPathItem.Operations.First();
        Assert.True(openApiOperation.HasParametersOrRequestBody() || openApiPathItem.HasParameters());

        // Construct SUT
        return new SyntaxGeneratorContractParameter(
            NullLogger.Instance,
            apiProject,
            openApiPathItem.Parameters,
            operationType,
            openApiOperation,
            FocusOnSegment);
    }

    [Theory(DisplayName = "Api Contract Parameter")]
    [MemberData(nameof(TestInput))]
    public Task ExecuteGeneratorTest(
        GeneratorTestInput input)
        => VerifyGeneratedOutput(input);
}
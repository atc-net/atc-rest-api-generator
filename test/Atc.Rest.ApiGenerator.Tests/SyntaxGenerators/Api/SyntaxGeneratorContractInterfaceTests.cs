namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

[UsesVerify]
public class SyntaxGeneratorContractInterfaceTests : SyntaxCodeGeneratorTestBase
{
    public static IEnumerable<object[]> TestInput { get; } = AllTestInput
        .Where(x => x.TestDirectory.Contains("ContractInterface", StringComparison.Ordinal))
        .Select(x => new object[] { x });

    protected override ISyntaxCodeGenerator CreateGenerator(
        ApiProjectOptions apiProject)
    {
        // Verify spec file supported for unit test
        Assert.Single(apiProject.Document.Paths);
        var urlPath = apiProject.Document.Paths.First();
        Assert.False(urlPath.IsPathStartingSegmentName(FocusOnSegment));
        Assert.Single(urlPath.Value.Operations);
        var (operationType, openApiOperation) = urlPath.Value.Operations.First();

        // Construct SUT
        return new SyntaxGeneratorContractInterface(
            apiProject,
            urlPath.Value.Parameters,
            operationType,
            openApiOperation,
            FocusOnSegment,
            urlPath.Value.HasParameters() || openApiOperation.HasParametersOrRequestBody());
    }

    [Theory(DisplayName = "Api Contract Interface")]
    [MemberData(nameof(TestInput))]
    public Task ExecuteGeneratorTest(
        GeneratorTestInput input)
        => VerifyGeneratedOutput(input);
}
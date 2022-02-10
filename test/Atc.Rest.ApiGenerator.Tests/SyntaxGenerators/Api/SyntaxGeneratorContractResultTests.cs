namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

[UsesVerify]
public class SyntaxGeneratorContractResultTests : SyntaxCodeGeneratorTestBase
{
    public static IEnumerable<object[]> TestInput { get; } = AllTestInput
        .Where(x => x.TestDirectory.Contains("ContractResult", StringComparison.Ordinal))
        .Select(x => new object[] { x });

    protected override ISyntaxCodeGenerator CreateGenerator(
        ApiProjectOptions apiProject)
    {
        // Verify spec file supported for unit test
        Assert.Single(apiProject.Document.Paths);
        var urlPath = apiProject.Document.Paths.First();
        Assert.False(urlPath.IsPathStartingSegmentName(FocusOnSegment));
        Assert.Single(urlPath.Value.Operations);
        var urlOperation = urlPath.Value.Operations.First();

        // Construct SUT
        return new SyntaxGeneratorContractResult(
            apiProject,
            urlOperation.Key,
            urlOperation.Value,
            FocusOnSegment);
    }

    [Theory(DisplayName = "Api Contract Result")]
    [MemberData(nameof(TestInput))]
    public Task ExecuteGeneratorTest(
        GeneratorTestInput input)
        => VerifyGeneratedOutput(input);
}
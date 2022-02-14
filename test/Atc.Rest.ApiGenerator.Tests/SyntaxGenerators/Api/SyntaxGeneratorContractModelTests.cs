namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

[UsesVerify]
public class SyntaxGeneratorContractModelTests : SyntaxCodeGeneratorTestBase
{
    public static IEnumerable<object[]> TestInput { get; } = AllTestInput
        .Where(x => x.TestDirectory.Contains("ContractModel", StringComparison.Ordinal))
        .Select(x => new object[] { x });

    protected override ISyntaxCodeGenerator CreateGenerator(
        ApiProjectOptions apiProject)
    {
        ArgumentNullException.ThrowIfNull(apiProject);

        // Verify spec file supported for unit test
        Assert.Single(apiProject.Document.Components.Schemas);
        var schema = apiProject.Document.Components.Schemas.First();

        // Construct SUT
        return new SyntaxGeneratorContractModel(NullLogger.Instance, apiProject, schema.Key, schema.Value, FocusOnSegment);
    }

    [Theory(DisplayName = "Api Contract Model")]
    [MemberData(nameof(TestInput))]
    public Task ExecuteGeneratorTest(
        GeneratorTestInput input)
        => VerifyGeneratedOutput(input);
}
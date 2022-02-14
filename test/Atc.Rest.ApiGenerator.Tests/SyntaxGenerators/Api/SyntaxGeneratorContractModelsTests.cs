namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

[UsesVerify]
public class SyntaxGeneratorContractModelsTests : SyntaxGeneratorContractModelsTestBase
{
    public static IEnumerable<object[]> TestInput { get; } = AllTestInput
        .Where(x => x.TestDirectory.Contains("ContractMultiModels", StringComparison.Ordinal))
        .Select(x => new object[] { x });

    protected override ISyntaxGeneratorContractModels CreateGenerator(
        ApiProjectOptions apiProject)
    {
        ArgumentNullException.ThrowIfNull(apiProject);

        // Verify spec file supported for unit test
        Assert.True(apiProject.Document.Components.Schemas.Count > 0);

        // Construct SUT
        var apiOperationSchemaMaps = apiProject.Document.Components.Schemas
            .Select(schema => new ApiOperationSchemaMap(schema.Key, SchemaMapLocatedAreaType.Response, FocusOnSegment, OperationType.Get, parentSchemaKey: null))
            .ToList();

        return new SyntaxGeneratorContractModels(NullLogger.Instance, apiProject, apiOperationSchemaMaps, FocusOnSegment);
    }

    [Theory(DisplayName = "Api Contract Model")]
    [MemberData(nameof(TestInput))]
    public Task ExecuteGeneratorTest(
        GeneratorTestInput input)
        => VerifyGeneratedOutput(input);
}
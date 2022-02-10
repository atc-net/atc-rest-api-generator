namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

[UsesVerify]
public class SyntaxGeneratorEndpointControllersTests : SyntaxCodeGeneratorTestBase
{
    public static IEnumerable<object[]> TestInput { get; } = AllTestInput
        .Where(x => x.TestDirectory.Contains("EndpointControllers", StringComparison.Ordinal))
        .Select(x => new object[] { x });

    protected override ISyntaxCodeGenerator CreateGenerator(
        ApiProjectOptions apiProject)
    {
        // Verify spec file supported for unit test
        Assert.Single(apiProject.BasePathSegmentNames);
        var segmentName = apiProject.BasePathSegmentNames.First();
        var operationSchemaMappings = OpenApiOperationSchemaMapHelper.CollectMappings(apiProject.Document);

        // Construct SUT
        return new SyntaxGeneratorEndpointControllers(apiProject, operationSchemaMappings, segmentName);
    }

    [Theory(DisplayName = "Api Contract Controllers")]
    [MemberData(nameof(TestInput))]
    public Task ExecuteGeneratorTest(
        GeneratorTestInput input)
        => VerifyGeneratedOutput(input);
}
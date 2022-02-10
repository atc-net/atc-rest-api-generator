namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api;

public abstract class SyntaxGeneratorContractModelsTestBase : GeneratorTestBase
{
    protected abstract ISyntaxGeneratorContractModels CreateGenerator(ApiProjectOptions apiProject);

    protected async Task VerifyGeneratedOutput(
        GeneratorTestInput input)
    {
        // Arrange
        var apiProject = await CreateApiProjectAsync(input);
        var verifySettings = CreateVerifySettings(input, apiProject);

        var sut = CreateGenerator(apiProject);

        // Act
        var generatedCode = GetGeneratedCode(sut);

        // Assert
        await VerifyGeneratedCode(generatedCode, verifySettings);
    }

    private static string GetGeneratedCode(
        ISyntaxGeneratorContractModels sut)
    {
        var syntaxGeneratorContractModels = sut.GenerateSyntaxTrees();

        var sb = new StringBuilder();
        for (var i = 0; i < syntaxGeneratorContractModels.Count; i++)
        {
            var syntaxGeneratorContractModel = syntaxGeneratorContractModels[i];
            sb.AppendLine(syntaxGeneratorContractModel.ToCodeAsString());
            if (i != syntaxGeneratorContractModels.Count - 1)
            {
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}
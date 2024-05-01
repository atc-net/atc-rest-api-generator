namespace Atc.CodeGeneration.CSharp.Content.Generators;

public class GenerateContentForSolutionFile : IContentGenerator
{
    private readonly SolutionFileParameters parameters;

    public GenerateContentForSolutionFile(
        SolutionFileParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        return sb.ToString();
    }
}

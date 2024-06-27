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

        sb.AppendLine();
        sb.AppendLine("Microsoft Visual Studio Solution File, Format Version 12.00");
        sb.AppendLine("# Visual Studio Version 17");
        sb.AppendLine("VisualStudioVersion = 17.0.31903.59");
        sb.AppendLine("MinimumVisualStudioVersion = 15.0.26124.0");

        foreach (var project in parameters.Projects)
        {
            sb.AppendLine($"Project(\"{project.ProjectTypeId}\") = \"{project.Name}\", \"{project.RelativePath}\", \"{project.ConfigurationId}\"");
            sb.AppendLine("EndProject");
        }

        sb.AppendLine("Global");
        sb.AppendLine(4, "GlobalSection(SolutionConfigurationPlatforms) = preSolution");
        sb.AppendLine(8, "Debug|Any CPU = Debug|Any CPU");
        sb.AppendLine(8, "Release|Any CPU = Release|Any CPU");
        sb.AppendLine(4, "EndGlobalSection");
        sb.AppendLine(4, "GlobalSection(ProjectConfigurationPlatforms) = postSolution");

        foreach (var configuration in parameters.Configurations)
        {
            sb.AppendLine(8, $"{{{configuration.ConfigurationId}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
            sb.AppendLine(8, $"{{{configuration.ConfigurationId}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
            sb.AppendLine(8, $"{{{configuration.ConfigurationId}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
            sb.AppendLine(8, $"{{{configuration.ConfigurationId}}}.Release|Any CPU.Build.0 = Release|Any CPU");
        }

        sb.AppendLine(4, "EndGlobalSection");
        sb.AppendLine(4, "GlobalSection(SolutionProperties) = preSolution");
        sb.AppendLine(8, "HideSolutionNode = FALSE");
        sb.AppendLine(4, "EndGlobalSection");
        sb.AppendLine(4, "GlobalSection(ExtensibilityGlobals) = postSolution");
        sb.AppendLine(8, $"SolutionGuid = {{{parameters.SolutionId}}}");
        sb.AppendLine(4, "EndGlobalSection");
        sb.AppendLine(4, "EndGlobal");

        return sb.ToString();
    }
}
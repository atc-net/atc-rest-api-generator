namespace Atc.CodeGeneration.CSharp.Content.Generators;

public class GenerateContentForSolutionDotSettingsFile : IContentGenerator
{
    [SuppressMessage("X", "S4487: Remove this unread private field 'xxx' or refactor the code to use its value", Justification = "OK - WIP")]
    private readonly SolutionDotSettingsFileParameters parameters;

    public GenerateContentForSolutionDotSettingsFile(
        SolutionDotSettingsFileParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<wpf:ResourceDictionary xml:space=\"preserve\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:s=\"clr-namespace:System;assembly=mscorlib\" xmlns:ss=\"urn:shemas-jetbrains-com:settings-storage-xaml\" xmlns:wpf=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">");

        sb.AppendLine("</wpf:ResourceDictionary>");
        return sb.ToString();
    }
}
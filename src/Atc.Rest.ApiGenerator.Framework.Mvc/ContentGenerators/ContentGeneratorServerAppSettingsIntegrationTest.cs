namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators;

public sealed class ContentGeneratorServerAppSettingsIntegrationTest : IContentGenerator
{
    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine("{");
        sb.Append('}');

        return sb.ToString();
    }
}
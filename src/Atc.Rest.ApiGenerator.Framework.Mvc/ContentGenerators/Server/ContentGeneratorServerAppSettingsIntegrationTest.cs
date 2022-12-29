namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerAppSettingsIntegrationTest : IContentGenerator
{
    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine("{");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
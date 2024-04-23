namespace Atc.Rest.ApiGenerator.Framework.Core.ContentGenerators.Server;

public sealed class ContentGeneratorServerWebConfig : IContentGenerator
{
    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        sb.AppendLine("<configuration>");
        sb.AppendLine("  <system.webServer>");
        sb.AppendLine("    <security>");
        sb.AppendLine("      <requestFiltering>");
        sb.AppendLine("        <requestLimits maxAllowedContentLength=\"2147483647\" />");
        sb.AppendLine("      </requestFiltering>");
        sb.AppendLine("    </security>");
        sb.AppendLine("  </system.webServer>");
        sb.AppendLine("</configuration>");

        return sb.ToString();
    }
}
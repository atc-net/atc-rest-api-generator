namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerProgram : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerProgram(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.AppendLine("public static class Program");
        sb.AppendLine("{");
        sb.AppendLine(4, "public static void Main(string[] args)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "CreateHostBuilder(args)");
        sb.AppendLine(12, ".Build()");
        sb.AppendLine(12, ".Run();");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public static IHostBuilder CreateHostBuilder(string[] args)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "var builder = Host");
        sb.AppendLine(12, ".CreateDefaultBuilder(args)");
        sb.AppendLine(12, ".ConfigureWebHostDefaults(webBuilder =>");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "webBuilder.UseStartup<Startup>();");
        sb.AppendLine(12, "});");
        sb.AppendLine();
        sb.AppendLine(8, "return builder;");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
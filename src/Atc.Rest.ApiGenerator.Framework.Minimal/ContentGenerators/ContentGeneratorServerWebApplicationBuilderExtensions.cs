namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators;

public class ContentGeneratorServerWebApplicationBuilderExtensions : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerWebApplicationBuilderExtensions(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace}.Extensions;"); // TODO: Move to constant
        sb.AppendLine();
        sb.AppendLine("public static class WebApplicationBuilderExtensions");
        sb.AppendLine("{");
        sb.AppendLine(4, "public static void ConfigureLogging(");
        sb.AppendLine(8, "this WebApplicationBuilder builder)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(builder);");
        sb.AppendLine();
        sb.AppendLine(8, "builder.Services.AddLogging(logging =>");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "logging.SetMinimumLevel(LogLevel.Information);");
        sb.AppendLine(12, "logging.AddConsole();");
        sb.AppendLine(12, "logging.AddDebug();");
        sb.AppendLine(8, "});");
        sb.AppendLine();
        sb.AppendLine(8, "builder.Services.AddHttpLogging(logging =>");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "logging.LoggingFields = HttpLoggingFields.All;");
        sb.AppendLine(12, "logging.RequestHeaders.Add(\"Authorization\");");
        sb.AppendLine(12, "logging.RequestBodyLogLimit = 4096;");
        sb.AppendLine(12, "logging.ResponseBodyLogLimit = 4096;");
        sb.AppendLine(12, "logging.MediaTypeOptions.AddText(\"application/json\");");
        sb.AppendLine(8, "});");
        sb.AppendLine();
        sb.AppendLine(8, "builder.Services.AddApplicationInsightsTelemetry();");
        sb.AppendLine();
        sb.AppendLine(8, "builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(");
        sb.AppendLine(12, "\"Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware\",");
        sb.AppendLine(12, "LogLevel.Trace);");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
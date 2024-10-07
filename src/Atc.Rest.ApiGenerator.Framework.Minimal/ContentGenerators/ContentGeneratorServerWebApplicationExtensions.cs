// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators;

public class ContentGeneratorServerWebApplicationExtensions : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerWebApplicationExtensions(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public SwaggerThemeMode SwaggerThemeMode { get; set; } = SwaggerThemeMode.None;

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace}.Extensions;"); // TODO: Move to constant
        sb.AppendLine();
        sb.AppendLine("public static class WebApplicationExtensions");
        sb.AppendLine("{");
        sb.AppendLine(4, "private static readonly string[] PatchHttpMethods = [\"patch\"];");
        sb.AppendLine();
        sb.AppendLine(4, "public static RouteHandlerBuilder MapPatch(");
        sb.AppendLine(8, "this WebApplication app,");
        sb.AppendLine(8, "string pattern,");
        sb.AppendLine(8, "Delegate handler)");
        sb.AppendLine(8, "=> app.MapMethods(");
        sb.AppendLine(12, "pattern,");
        sb.AppendLine(12, "PatchHttpMethods,");
        sb.AppendLine(12, "handler);");

        if (SwaggerThemeMode != SwaggerThemeMode.None)
        {
            sb.AppendLine();
            sb.AppendLine(4, "public static IApplicationBuilder ConfigureSwagger(");
            sb.AppendLine(8, "this WebApplication app,");
            sb.AppendLine(8, "string applicationName)");
            sb.AppendLine(4, "{");
            sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(app);");
            sb.AppendLine();
            sb.AppendLine(8, "if (!app.Environment.IsDevelopment())");
            sb.AppendLine(8, "{");
            sb.AppendLine(12, "return app;");
            sb.AppendLine(8, "}");
            sb.AppendLine();
            sb.AppendLine(8, "app.UseSwagger();");
            sb.AppendLine(8, "app.UseSwaggerUI(options =>");
            sb.AppendLine(8, "{");
            sb.AppendLine(12, "options.EnableTryItOutByDefault();");

            switch (SwaggerThemeMode)
            {
                case SwaggerThemeMode.Dark:
                    sb.AppendLine(12, "options.InjectStylesheet(\"/swagger-ui/SwaggerDark.css\");");
                    break;
                case SwaggerThemeMode.Light:
                    sb.AppendLine(12, "options.InjectStylesheet(\"/swagger-ui/SwaggerLight.css\");");
                    break;
            }

            sb.AppendLine(12, "options.InjectJavascript(\"/swagger-ui/main.js\");");
            sb.AppendLine();

            sb.AppendLine(12, "var descriptions = app.DescribeApiVersions();");
            sb.AppendLine(12, "foreach (var (url, name) in");
            sb.AppendLine(20, "from description in descriptions");
            sb.AppendLine(20, "let url = $\"/swagger/{description.GroupName}/swagger.json\"");
            sb.AppendLine(20, "let name = description.GroupName.ToUpperInvariant()");
            sb.AppendLine(20, "select (url, name))");
            sb.AppendLine(12, "{");
            sb.AppendLine(16, "options.SwaggerEndpoint(url, $\"{applicationName} {name}\");");
            sb.AppendLine(12, "}");
            sb.AppendLine(8, "});");
            sb.AppendLine();
            sb.AppendLine(4, "    return app;");
            sb.AppendLine(4, "}");
        }
        sb.Append('}');

        return sb.ToString();
    }
}
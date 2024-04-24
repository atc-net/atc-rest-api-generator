namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators.Server;

public class ContentGeneratorServeConfigureSwaggerOptions : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServeConfigureSwaggerOptions(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.AppendLine("public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>");
        sb.AppendLine("{");
        sb.AppendLine(4, "private readonly IApiVersionDescriptionProvider provider;");
        sb.AppendLine(4, "private readonly IWebHostEnvironment environment;");
        sb.AppendLine();
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, "/// Initializes a new instance of the <see cref=\"ConfigureSwaggerOptions\"/> class.");
        sb.AppendLine(4, "/// </summary>");
        sb.AppendLine(4, "/// <param name=\"provider\">The <see cref=\"IApiVersionDescriptionProvider\">provider</see> used to generate Swagger documents.</param>");
        sb.AppendLine(4, "/// <param name=\"environment\">The environment.</param>");
        sb.AppendLine(4, "public ConfigureSwaggerOptions(");
        sb.AppendLine(8, "IApiVersionDescriptionProvider provider,");
        sb.AppendLine(8, "IWebHostEnvironment environment)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "this.provider = provider;");
        sb.AppendLine(8, "this.environment = environment;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "/// <inheritdoc />");
        sb.AppendLine(4, "public void Configure(");
        sb.AppendLine(8, "SwaggerGenOptions options)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "// Add a swagger document for each discovered API version");
        sb.AppendLine(8, "// note: you might choose to skip or document deprecated API versions differently");
        sb.AppendLine(8, "foreach (var description in provider.ApiVersionDescriptions)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));");
        sb.AppendLine(8, "}");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "private OpenApiInfo CreateInfoForApiVersion(");
        sb.AppendLine(8, "ApiVersionDescription description)");
        sb.AppendLine(4, "{");

        // TODO: Add fix details to the OpenApiInfo
        sb.AppendLine(8, "var text = new StringBuilder(\"An example API to showcase minimal api implementation using the Atc.Rest.MinimalApi Nuget package.\");");
        sb.AppendLine(8, "var info = new OpenApiInfo");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "Title = $\"{ environment.ApplicationName} {description.GroupName.ToUpperInvariant()}\",");
        sb.AppendLine(12, "Version = description.ApiVersion.ToString(),");
        sb.AppendLine(12, "Contact = new OpenApiContact { Name = \"atc-net\", Email = \"atcnet.org@gmail.com\" },");
        sb.AppendLine(8, "};");
        sb.AppendLine();
        sb.AppendLine(8, "if (description.IsDeprecated)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "text.Append(\" This API version has been deprecated.\");");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "if (description.SunsetPolicy is { } policy)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "if (policy.Date is { } when)");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "text.Append(\" The API will be sunset on \")");
        sb.AppendLine(20, ".Append(when.Date.ToShortDateString())");
        sb.AppendLine(20, ".Append('.');");
        sb.AppendLine(12, "}");
        sb.AppendLine();
        sb.AppendLine(12, "if (policy.HasLinks)");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "text.AppendLine();");
        sb.AppendLine();
        sb.AppendLine(16, "foreach (var link in policy.Links)");
        sb.AppendLine(16, "{");
        sb.AppendLine(20, "if (link.Type != \"text /html\")");
        sb.AppendLine(20, "{");
        sb.AppendLine(24, "continue;");
        sb.AppendLine(20, "}");
        sb.AppendLine();
        sb.AppendLine(20, "text.AppendLine();");
        sb.AppendLine();
        sb.AppendLine(20, "if (link.Title.HasValue)");
        sb.AppendLine(20, "{");
        sb.AppendLine(24, "text.Append(link.Title.Value).Append(\": \");");
        sb.AppendLine(20, "}");
        sb.AppendLine();
        sb.AppendLine(20, "text.Append(link.LinkTarget.OriginalString);");
        sb.AppendLine(16, "}");
        sb.AppendLine(12, "}");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "info.Description = text.ToString();");
        sb.AppendLine();
        sb.AppendLine(8, "return info;");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
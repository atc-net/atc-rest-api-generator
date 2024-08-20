namespace Atc.Rest.ApiGenerator.Framework.ContentGenerators.Server;

public sealed class ContentGeneratorServerConfigureSwaggerDocOptions : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerSwaggerDocOptionsParameters parameters;

    public ContentGeneratorServerConfigureSwaggerDocOptions(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerSwaggerDocOptionsParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace}.Options;"); // TODO: Move to constant
        sb.AppendLine();
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine("public class ConfigureSwaggerDocOptions : IConfigureOptions<SwaggerGenOptions>");
        sb.AppendLine("{");
        sb.AppendLine(4, "private readonly IApiVersionDescriptionProvider provider;");
        sb.AppendLine(4, "private readonly IWebHostEnvironment environment;");
        sb.AppendLine();
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, "/// Initializes a new instance of the <see cref=\"ConfigureSwaggerDocOptions\"/> class.");
        sb.AppendLine(4, "/// </summary>");
        sb.AppendLine(4, "/// <param name=\"provider\">The <see cref=\"IApiVersionDescriptionProvider\">provider</see> used to generate Swagger documents.</param>");
        sb.AppendLine(4, "/// <param name=\"environment\">The environment.</param>");
        sb.AppendLine(4, "public ConfigureSwaggerDocOptions(");
        sb.AppendLine(8, "IApiVersionDescriptionProvider provider,");
        sb.AppendLine(8, "IWebHostEnvironment environment)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "this.provider = provider;");
        sb.AppendLine(8, "this.environment = environment;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public void Configure(");
        sb.AppendLine(8, "SwaggerGenOptions options)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "foreach (var description in provider.ApiVersionDescriptions)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, \"xml\"));");
        sb.AppendLine(4, "}");

        sb.AppendLine();
        sb.AppendLine(4, "private OpenApiInfo CreateInfoForApiVersion(");
        sb.AppendLine(8, "ApiVersionDescription description)");
        sb.AppendLine(4, "{");

        var description = string.Empty;
        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.Description))
        {
            description = parameters.SwaggerDocOptions.Description!
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace("'", string.Empty, StringComparison.Ordinal)
                .Trim();
        }

        sb.AppendLine(8, $"var text = new StringBuilder(\"{description}\");");
        sb.AppendLine(8, "var info = new OpenApiInfo");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "Title = $\"{environment.ApplicationName} {description.GroupName.ToUpperInvariant()}\",");
        sb.AppendLine(12, "Version = description.ApiVersion.ToString(),");
        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactName) ||
            !string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactEmail) ||
            !string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactUrl))
        {
            sb.AppendLine(12, "Contact = new OpenApiContact");
            sb.AppendLine(12, "{");

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactName))
            {
                sb.AppendLine(16, $"Name = \"{parameters.SwaggerDocOptions.ContactName}\",");
            }

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactEmail))
            {
                sb.AppendLine(16, $"Email = \"{parameters.SwaggerDocOptions.ContactEmail}\",");
            }

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactUrl))
            {
                sb.AppendLine(16, $"Url = new Uri(\"{parameters.SwaggerDocOptions.ContactUrl}\"),");
            }

            sb.AppendLine(12, "},");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.TermsOfService))
        {
            sb.AppendLine(12, $"TermsOfService = new Uri(\"{parameters.SwaggerDocOptions.TermsOfService}\"),");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseName) ||
            !string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseUrl))
        {
            sb.AppendLine(12, "License = new OpenApiLicense");
            sb.AppendLine(12, "{");

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseName))
            {
                sb.AppendLine(16, $"Name = \"{parameters.SwaggerDocOptions.LicenseName}\",");
            }

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseUrl))
            {
                sb.AppendLine(16, $"Url = new Uri(\"{parameters.SwaggerDocOptions.LicenseUrl}\"),");
            }

            sb.AppendLine(12, "},");
        }

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
        sb.AppendLine(20, "if (link.Type != \"text/html\")");
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
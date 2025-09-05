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
        sb.AppendLine($"namespace {parameters.Namespace}.Options;");
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

        var opts = parameters.SwaggerDocOptions;
        var descriptionText = string.Empty;
        if (!string.IsNullOrWhiteSpace(opts.Description))
        {
            descriptionText = opts.Description!
                .Replace("\"", string.Empty, StringComparison.Ordinal)
                .Replace("'", string.Empty, StringComparison.Ordinal)
                .Trim();
        }

        AppendStringBuilderInit(sb, 8, descriptionText);
        AppendApiInfo(sb, 8, opts);
        AppendSunsetPolicyBlock(sb, 8);

        sb.AppendLine(8, "info.Description = text.ToString();");
        sb.AppendLine();
        sb.AppendLine(8, "return info;");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }

    private static void AppendApiInfo(
        StringBuilder sb,
        int baseIndent,
        SwaggerDocOptionsParameters opts)
    {
        var indent = baseIndent + 4;

        sb.AppendLine(baseIndent, "var info = new OpenApiInfo");
        sb.AppendLine(baseIndent, "{");
        sb.AppendLine(indent, "Title = $\"{environment.ApplicationName} {description.GroupName.ToUpperInvariant()}\",");
        sb.AppendLine(indent, "Version = description.ApiVersion.ToString(),");

        AppendContact(sb, indent, opts.ContactName, opts.ContactEmail, opts.ContactUrl);
        AppendTermsOfService(sb, indent, opts.TermsOfService);
        AppendLicense(sb, indent, opts.LicenseName, opts.LicenseUrl);

        sb.AppendLine(baseIndent, "};");
        sb.AppendLine();
    }

    private static void AppendSunsetPolicyBlock(
        StringBuilder sb,
        int baseIndent)
    {
        var i1 = baseIndent + 4;
        var i2 = i1 + 4;
        var i3 = i2 + 4;
        var i4 = i3 + 4;

        sb.AppendLine(baseIndent, "if (description.IsDeprecated)");
        sb.AppendLine(baseIndent, "{");
        sb.AppendLine(i1, "text.Append(\" This API version has been deprecated.\");");
        sb.AppendLine(baseIndent, "}");
        sb.AppendLine();
        sb.AppendLine(baseIndent, "if (description.SunsetPolicy is { } policy)");
        sb.AppendLine(baseIndent, "{");
        sb.AppendLine(i1, "if (policy.Date is { } when)");
        sb.AppendLine(i1, "{");
        sb.AppendLine(i2, "text.Append(\" The API will be sunset on \")");
        sb.AppendLine(i3, ".Append(when.Date.ToShortDateString())");
        sb.AppendLine(i3, ".Append('.');");
        sb.AppendLine(i1, "}");
        sb.AppendLine();
        sb.AppendLine(i1, "if (policy.HasLinks)");
        sb.AppendLine(i1, "{");
        sb.AppendLine(i2, "text.AppendLine();");
        sb.AppendLine();
        sb.AppendLine(i2, "foreach (var link in policy.Links)");
        sb.AppendLine(i2, "{");
        sb.AppendLine(i3, "if (link.Type != \"text/html\")");
        sb.AppendLine(i3, "{");
        sb.AppendLine(i4, "continue;");
        sb.AppendLine(i3, "}");
        sb.AppendLine();
        sb.AppendLine(i3, "text.AppendLine();");
        sb.AppendLine();
        sb.AppendLine(i3, "if (link.Title.HasValue)");
        sb.AppendLine(i3, "{");
        sb.AppendLine(i4, "text.Append(link.Title.Value).Append(\": \");");
        sb.AppendLine(i3, "}");
        sb.AppendLine();
        sb.AppendLine(i3, "text.Append(link.LinkTarget.OriginalString);");
        sb.AppendLine(i2, "}");
        sb.AppendLine(i1, "}");
        sb.AppendLine(baseIndent, "}");
        sb.AppendLine();
    }

    private static void AppendStringBuilderInit(
        StringBuilder sb,
        int baseIndent,
        string? text)
    {
        sb.Append(baseIndent, "var text = new StringBuilder(");
        if (!string.IsNullOrEmpty(text))
        {
            sb.Append(BuildStringLiteral(text));
        }
        sb.Append(");");
        sb.AppendLine();
    }

    private static void AppendContact(
        StringBuilder sb,
        int baseIndent,
        string? name,
        string? email,
        string? url)
    {
        if (!Has(name) &&
            !Has(email) &&
            !Has(url))
        {
            return;
        }

        var innerIndent = baseIndent + 4;

        sb.AppendLine(baseIndent, "Contact = new OpenApiContact");
        sb.AppendLine(baseIndent, "{");
        if (Has(name))
        {
            sb.AppendLine(innerIndent, $"Name = {BuildStringLiteral(name!)},");
        }

        if (Has(email))
        {
            sb.AppendLine(innerIndent, $"Email = {BuildStringLiteral(email!)},");
        }

        if (Has(url))
        {
            sb.AppendLine(innerIndent, $"Url = new Uri({BuildStringLiteral(url!)}),");
        }

        sb.AppendLine(baseIndent, "},");
    }

    private static void AppendTermsOfService(
        StringBuilder sb,
        int indentBase,
        string? termsUrl)
    {
        if (Has(termsUrl))
        {
            sb.AppendLine(indentBase, $"TermsOfService = new Uri({BuildStringLiteral(termsUrl!)}),");
        }
    }

    private static void AppendLicense(
        StringBuilder sb,
        int indentBase,
        string? licenseName,
        string? licenseUrl)
    {
        if (!Has(licenseName) &&
            !Has(licenseUrl))
        {
            return;
        }

        var innerIndent = indentBase + 4;

        sb.AppendLine(indentBase, "License = new OpenApiLicense");
        sb.AppendLine(indentBase, "{");
        if (Has(licenseName))
        {
            sb.AppendLine(innerIndent, $"Name = {BuildStringLiteral(licenseName!)},");
        }

        if (Has(licenseUrl))
        {
            sb.AppendLine(innerIndent, $"Url = new Uri({BuildStringLiteral(licenseUrl!)}),");
        }

        sb.AppendLine(indentBase, "},");
    }

    private static string BuildStringLiteral(
        string text)
    {
        // Use verbatim if backslashes or newlines are present.
        if (text.IndexOfAny(new[] { '\\', '\r', '\n' }) >= 0)
        {
            var verbatim = text.Replace("\"", "\"\"", StringComparison.Ordinal);
            return "@\"" + verbatim + "\"";
        }

        // Otherwise, use a normal C# string literal.
        var normal = text
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);
        return "\"" + normal + "\"";
    }

    private static bool Has(
        string? s)
        => !string.IsNullOrWhiteSpace(s);
}
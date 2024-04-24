namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerSwaggerDocOptions : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerSwaggerDocOptionsParameters parameters;

    public ContentGeneratorServerSwaggerDocOptions(
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
        sb.AppendLine();
        sb.AppendLine(4, "public ConfigureSwaggerDocOptions(");
        sb.AppendLine(8, "IApiVersionDescriptionProvider provider)");
        sb.AppendLine(8, "=> this.provider = provider;");
        sb.AppendLine();
        sb.AppendLine(4, "public void Configure(");
        sb.AppendLine(8, "SwaggerGenOptions options)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "foreach (var version in provider.ApiVersionDescriptions)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "options.SwaggerDoc(");
        sb.AppendLine(16, "version.GroupName,");
        sb.AppendLine(16, "new OpenApiInfo");
        sb.AppendLine(16, "{");

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.Version))
        {
            sb.AppendLine(20, $"Version = \"{parameters.SwaggerDocOptions.Version}\",");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.Title))
        {
            sb.AppendLine(20, $"Title = \"{parameters.SwaggerDocOptions.Title}\",");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.Description))
        {
            sb.AppendLine(20, $"Description = \"{parameters.SwaggerDocOptions.Description}\",");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactName) ||
            !string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactEmail) ||
            !string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactUrl))
        {
            sb.AppendLine(20, "Contact = new OpenApiContact");
            sb.AppendLine(20, "{");

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactName))
            {
                sb.AppendLine(24, $"Name = \"{parameters.SwaggerDocOptions.ContactName}\",");
            }

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactEmail))
            {
                sb.AppendLine(24, $"Email = \"{parameters.SwaggerDocOptions.ContactEmail}\",");
            }

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.ContactUrl))
            {
                sb.AppendLine(24, $"Url = new Uri(\"{parameters.SwaggerDocOptions.ContactUrl}\"),");
            }

            sb.AppendLine(20, "},");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.TermsOfService))
        {
            sb.AppendLine(20, $"TermsOfService = new Uri(\"{parameters.SwaggerDocOptions.TermsOfService}\"),");
        }

        if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseName) ||
            !string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseUrl))
        {
            sb.AppendLine(20, "License = new OpenApiLicense");
            sb.AppendLine(20, "{");

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseName))
            {
                sb.AppendLine(24, $"Name = \"{parameters.SwaggerDocOptions.LicenseName}\",");
            }

            if (!string.IsNullOrWhiteSpace(parameters.SwaggerDocOptions.LicenseUrl))
            {
                sb.AppendLine(24, $"Url = new Uri(\"{parameters.SwaggerDocOptions.LicenseUrl}\"),");
            }

            sb.AppendLine(20, "},");
        }

        sb.AppendLine(16, "});");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, \"xml\"));");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
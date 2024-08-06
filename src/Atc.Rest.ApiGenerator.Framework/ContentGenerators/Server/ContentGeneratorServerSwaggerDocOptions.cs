namespace Atc.Rest.ApiGenerator.Framework.ContentGenerators.Server;

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
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, "/// Initializes a new instance of the <see cref=\"ConfigureSwaggerOptions\"/> class.");
        sb.AppendLine(4, "/// </summary>");
        sb.AppendLine(4, "/// <param name=\"provider\">The <see cref=\"IApiVersionDescriptionProvider\">provider</see> used to generate Swagger documents.</param>");
        sb.AppendLine(4, "public ConfigureSwaggerDocOptions(");
        sb.AppendLine(8, "IApiVersionDescriptionProvider provider)");
        sb.AppendLine(8, "=> this.provider = provider;");
        sb.AppendLine();
        sb.AppendLine(4, "public void Configure(");
        sb.AppendLine(8, "SwaggerGenOptions options)");
        sb.AppendLine(8, "=> options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, \"xml\"));");
        sb.Append('}');

        return sb.ToString();
    }
}
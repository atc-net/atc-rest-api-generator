namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerWebApiStartupFactory : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerWebApiStartupFactoryParameters parameters;

    public ContentGeneratorServerWebApiStartupFactory(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerWebApiStartupFactoryParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine("public partial class WebApiStartupFactory : WebApplicationFactory<Startup>");
        sb.AppendLine("{");
        sb.AppendLine(4, "protected override void ConfigureWebHost(IWebHostBuilder builder)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "builder.ConfigureAppConfiguration(config =>");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "ModifyConfiguration(config);");
        sb.AppendLine(12, "var integrationConfig = new ConfigurationBuilder()");
        sb.AppendLine(16, ".AddJsonFile(\"appsettings.integrationtest.json\")");
        sb.AppendLine(16, ".Build();");
        sb.AppendLine(12, "config.AddConfiguration(integrationConfig);");
        sb.AppendLine(8, "});");
        sb.AppendLine();
        sb.AppendLine(8, "builder.ConfigureTestServices(services =>");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "ModifyServices(services);");
        sb.AppendLine(12, "services.AddSingleton<RestApiOptions, RestApiOptions>();");
        sb.AppendLine(12, "services.AutoRegistrateServices(");
        sb.AppendLine(16, "Assembly.GetAssembly(typeof(ApiRegistration))!,");
        sb.AppendLine(16, "Assembly.GetAssembly(typeof(WebApiStartupFactory))!);");
        sb.AppendLine(8, "});");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "partial void ModifyConfiguration(IConfigurationBuilder config);");
        sb.AppendLine();
        sb.AppendLine(4, "partial void ModifyServices(IServiceCollection services);");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
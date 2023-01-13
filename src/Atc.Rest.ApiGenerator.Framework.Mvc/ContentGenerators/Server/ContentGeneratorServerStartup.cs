namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerStartup : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerStartup(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.AppendLine("public class Startup");
        sb.AppendLine("{");
        sb.AppendLine(4, "private readonly RestApiExtendedOptions restApiOptions;");
        sb.AppendLine();
        sb.AppendLine(4, "public Startup(");
        sb.AppendLine(8, "IConfiguration configuration)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "Configuration = configuration;");
        sb.AppendLine(8, "restApiOptions = new RestApiExtendedOptions");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "// TODO: Remove/out-comment/set to false this for production scenarios!");
        sb.AppendLine(12, "AllowAnonymousAccessForDevelopment = true,");
        sb.AppendLine(8, "};");
        sb.AppendLine();
        sb.AppendLine(8, "restApiOptions.AddAssemblyPairs(");
        sb.AppendLine(12, "Assembly.GetAssembly(typeof(ApiRegistration)),");
        sb.AppendLine(12, "Assembly.GetAssembly(typeof(DomainRegistration)));");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public IConfiguration Configuration { get; }");
        sb.AppendLine();
        sb.AppendLine(4, "public void ConfigureServices(");
        sb.AppendLine(8, "IServiceCollection services)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "services.ConfigureOptions<ConfigureSwaggerDocOptions>();");
        sb.AppendLine();
        sb.AppendLine(8, "services.AddRestApi<Startup>(restApiOptions, Configuration);");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public void Configure(");
        sb.AppendLine(8, "IApplicationBuilder app,");
        sb.AppendLine(8, "IWebHostEnvironment env)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "app.ConfigureRestApi(env, restApiOptions);");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
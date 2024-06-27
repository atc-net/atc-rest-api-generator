namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators;

public sealed class ContentGeneratorServerProgram : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerProgram(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public SwaggerThemeMode SwaggerThemeMode { get; set; } = SwaggerThemeMode.None;

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.AppendLine("public static class Program");
        sb.AppendLine("{");
        sb.AppendLine(4, "[SuppressMessage(\"Design\", \"MA0051:Method is too long\", Justification = \"OK.\")]");
        sb.AppendLine(4, "public static void Main(string[] args)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "var builder = WebApplication.CreateBuilder(args);");
        sb.AppendLine();
        sb.AppendLine(8, "builder.ConfigureLogging();");
        sb.AppendLine();
        sb.AppendLine(8, "var services = builder.Services;");
        sb.AppendLine();
        sb.AppendLine(8, "services.AddMemoryCache();");
        sb.AppendLine();
        sb.AppendLine(8, "services.ConfigureDomainServices(builder.Configuration);");
        sb.AppendLine();
        sb.AppendLine(8, "services.AddValidatorsFromAssemblyContaining<IDomainAssemblyMarker>(ServiceLifetime.Singleton);");
        sb.AppendLine();
        sb.AppendLine(8, "services.ConfigureApiVersioning();");
        sb.AppendLine();
        sb.AppendLine(8, "services.AddEndpointDefinitions(typeof(IApiContractAssemblyMarker));");
        sb.AppendLine();

        if (SwaggerThemeMode != SwaggerThemeMode.None)
        {
            sb.AppendLine(8, "services.ConfigureSwagger();");
            sb.AppendLine();
        }

        sb.AppendLine(8, "services.AddCors(corsOptions =>");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "corsOptions.AddPolicy(\"DemoCorsPolicy\", configurePolicy =>");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "configurePolicy");
        sb.AppendLine(20, ".AllowAnyOrigin()");
        sb.AppendLine(20, ".AllowAnyMethod()");
        sb.AppendLine(20, ".AllowAnyHeader();");
        sb.AppendLine(12, "});");
        sb.AppendLine(8, "});");
        sb.AppendLine();
        sb.AppendLine(8, "// This enables proper enum as string in Swagger UI");
        sb.AppendLine(8, "services.AddControllers().AddJsonOptions(o => JsonSerializerOptionsFactory.Create().Configure(o));");
        sb.AppendLine(8, "services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(o => JsonSerializerOptionsFactory.Create().Configure(o));");
        sb.AppendLine();
        sb.AppendLine(8, "services.AddSingleton(_ => new ValidationFilterOptions");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "SkipFirstLevelOnValidationKeys = true,");
        sb.AppendLine(8, "});");
        sb.AppendLine();
        sb.AppendLine(8, "var app = builder.Build();");
        sb.AppendLine();
        sb.AppendLine(8, "app.UseEndpointDefinitions();");
        sb.AppendLine();
        sb.AppendLine(8, "app.UseGlobalErrorHandler();");
        sb.AppendLine();
        sb.AppendLine(8, "// Enabling the status code pages middleware, will allow Problem Details to be used in some extra non-exception related framework scenarios,");
        sb.AppendLine(8, "// such as a 404 occurring due to a non-existent route or a 405 occurring due to a caller using an invalid HTTP method on an existing endpoint.");
        sb.AppendLine(8, "app.UseStatusCodePages();");
        sb.AppendLine();
        sb.AppendLine(8, "app.UseStaticFiles();");
        sb.AppendLine();

        if (SwaggerThemeMode != SwaggerThemeMode.None)
        {
            sb.AppendLine(8, "app.ConfigureSwagger(builder.Environment.ApplicationName);");
        }

        sb.AppendLine();
        sb.AppendLine(8, "app.UseHttpsRedirection();");
        sb.AppendLine(8, "app.UseHsts();");
        sb.AppendLine();
        sb.AppendLine(8, "app.UseCors(\"DemoCorsPolicy\");");
        sb.AppendLine();
        sb.AppendLine(8, "if (!app.Environment.IsDevelopment())");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "app.UseAuthentication();");
        sb.AppendLine(12, "app.UseAuthorization();");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "app.Run();");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
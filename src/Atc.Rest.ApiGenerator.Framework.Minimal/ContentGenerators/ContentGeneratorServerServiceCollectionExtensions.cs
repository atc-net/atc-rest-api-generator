namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators;

public class ContentGeneratorServerServiceCollectionExtensions : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerServiceCollectionExtensions(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace}.Extensions;"); // TODO: Move to constant
        sb.AppendLine();
        sb.AppendLine("public static class ServiceCollectionExtensions");
        sb.AppendLine("{");
        sb.AppendLine(4, "public static void ConfigureApiVersioning(");
        sb.AppendLine(8, "this IServiceCollection services)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "services.AddEndpointsApiExplorer();");
        sb.AppendLine(8, "services");
        sb.AppendLine(12, ".AddApiVersioning(options =>");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "// Specify the default API Version");
        sb.AppendLine(16, "options.DefaultApiVersion = new ApiVersion(1, 0);");
        sb.AppendLine();
        sb.AppendLine(16, "// If the client hasn't specified the API version in the request, use the default API version number");
        sb.AppendLine(16, "options.AssumeDefaultVersionWhenUnspecified = true;");
        sb.AppendLine();
        sb.AppendLine(16, "// reporting api versions will return the headers");
        sb.AppendLine(16, "// \"api-supported-versions\" and \"api-deprecated-versions\"");
        sb.AppendLine(16, "options.ReportApiVersions = true;");
        sb.AppendLine();
        sb.AppendLine(16, "//// DEFAULT Version reader is QueryStringApiVersionReader();");
        sb.AppendLine(16, "//// clients request the specific version using the x-api-version header");
        sb.AppendLine(16, "//// Supporting multiple versioning scheme");
        sb.AppendLine(16, "options.ApiVersionReader = ApiVersionReader.Combine(");
        sb.AppendLine(20, "new HeaderApiVersionReader(ApiVersionConstants.ApiVersionHeaderParameter),");
        sb.AppendLine(20, "new MediaTypeApiVersionReader(ApiVersionConstants.ApiVersionMediaTypeParameter),");
        sb.AppendLine(20, "new QueryStringApiVersionReader(ApiVersionConstants.ApiVersionQueryParameter),");
        sb.AppendLine(20, "new QueryStringApiVersionReader(ApiVersionConstants.ApiVersionQueryParameterShort),");
        sb.AppendLine(20, "new UrlSegmentApiVersionReader());");
        sb.AppendLine(12, "})");
        sb.AppendLine(12, ".AddApiExplorer(options =>");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "// add the versioned api explorer, which also adds IApiVersionDescriptionProvider service");
        sb.AppendLine(16, "// note: the specified format code will format the version as \'v'major[.minor][-status]\"");
        sb.AppendLine(16, "options.GroupNameFormat = \"'v'VVV\";");
        sb.AppendLine();
        sb.AppendLine(16, "// note: this option is only necessary when versioning by url segment. The SubstitutionFormat");
        sb.AppendLine(16, "// can also be used to control the format of the API version in route templates");
        sb.AppendLine(16, "options.SubstituteApiVersionInUrl = true;");
        sb.AppendLine(12, "});");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public static void ConfigureSwagger(");
        sb.AppendLine(8, "this IServiceCollection services)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "services.ConfigureOptions<ConfigureSwaggerDocOptions>();");
        sb.AppendLine();
        sb.AppendLine(8, "services.AddSwaggerGen(options =>");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "options.OperationFilter<SwaggerDefaultValues>();");
        sb.AppendLine(12, "options.DocumentFilter<SwaggerEnumDescriptionsDocumentFilter>();");
        sb.AppendLine(8, "});");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
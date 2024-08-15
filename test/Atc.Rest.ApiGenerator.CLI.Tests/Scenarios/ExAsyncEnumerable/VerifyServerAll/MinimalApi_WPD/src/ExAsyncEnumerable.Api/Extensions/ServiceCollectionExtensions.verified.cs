namespace ExAsyncEnumerable.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static void ConfigureApiVersioning(
        this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(
                options =>
                {
                    // Specify the default API Version
                    options.DefaultApiVersion = new ApiVersion(1, 0);

                    // If the client hasn't specified the API version in the request, use the default API version number
                    options.AssumeDefaultVersionWhenUnspecified = true;

                    // reporting api versions will return the headers
                    // "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;

                    //// DEFAULT Version reader is QueryStringApiVersionReader();
                    //// clients request the specific version using the x-api-version header
                    //// Supporting multiple versioning scheme
                    options.ApiVersionReader = ApiVersionReader.Combine(
                        new HeaderApiVersionReader(ApiVersionConstants.ApiVersionHeaderParameter),
                        new MediaTypeApiVersionReader(ApiVersionConstants.ApiVersionMediaTypeParameter),
                        new QueryStringApiVersionReader(ApiVersionConstants.ApiVersionQueryParameter),
                        new QueryStringApiVersionReader(ApiVersionConstants.ApiVersionQueryParameterShort),
                        new UrlSegmentApiVersionReader());
                })
            .AddApiExplorer(options =>
            {
                // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                // note: the specified format code will format the version as 'v'major[.minor][-status]"
                options.GroupNameFormat = "'v'VVV";

                // note: this option is only necessary when versioning by url segment. The SubstitutionFormat
                // can also be used to control the format of the API version in route templates
                options.SubstituteApiVersionInUrl = true;
            });
        }

    public static void ConfigureSwagger(
        this IServiceCollection services)
    {
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<SwaggerDefaultValues>();
            options.DocumentFilter<SwaggerEnumDescriptionsDocumentFilter>();
        });
    }
}
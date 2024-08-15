namespace ExAsyncEnumerable.Api;

public class Startup
{
    private readonly RestApiExtendedOptions restApiOptions;

    public Startup(
        IConfiguration configuration)
    {
        Configuration = configuration;
        restApiOptions = new RestApiExtendedOptions
        {
            // TODO: Remove/out-comment/set to false this for production scenarios!
            AllowAnonymousAccessForDevelopment = true,
        };

        restApiOptions.AddAssemblyPairs(
            Assembly.GetAssembly(typeof(ApiRegistration)),
            Assembly.GetAssembly(typeof(DomainRegistration)));
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(
        IServiceCollection services)
    {
        services.ConfigureOptions<ConfigureSwaggerDocOptions>();

        services.AddRestApi<Startup>(restApiOptions, Configuration);
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        app.ConfigureRestApi(env, restApiOptions);
    }
}
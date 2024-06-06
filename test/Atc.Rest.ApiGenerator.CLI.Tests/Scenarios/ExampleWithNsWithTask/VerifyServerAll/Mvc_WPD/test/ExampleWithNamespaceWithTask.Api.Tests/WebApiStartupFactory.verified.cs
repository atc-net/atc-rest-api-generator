//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExampleWithNsWithTask.Api.Tests;

/// <summary>
/// Factory for bootstrapping in memory tests.
/// Includes options to override configuration and service collection using a partial class.
/// </summary>
[GeneratedCode("ApiGenerator", "x.x.x.x")]
public partial class WebApiStartupFactory : WebApplicationFactory<Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(config =>
        {
            ModifyConfiguration(config);
            var integrationConfig = new ConfigurationBuilder()
                .AddJsonFile("appsettings.integrationtest.json")
                .Build();
            config.AddConfiguration(integrationConfig);
        });

        builder.ConfigureTestServices(services =>
        {
            ModifyServices(services);
            services.AddSingleton<RestApiOptions, RestApiOptions>();
            services.AutoRegistrateServices(
                Assembly.GetAssembly(typeof(ApiRegistration))!,
                Assembly.GetAssembly(typeof(WebApiStartupFactory))!);
        });
    }

    partial void ModifyConfiguration(IConfigurationBuilder config);

    partial void ModifyServices(IServiceCollection services);
}

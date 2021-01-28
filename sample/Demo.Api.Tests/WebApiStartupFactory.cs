using System.CodeDom.Compiler;
using System.Reflection;
using Atc.Rest.Options;
using Demo.Api.Generated;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.41.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests
{
    /// <summary>
    /// Factory for bootstrapping in memory tests.
    ///
    /// Includes options to override configuration and service collection using a partial class.
    /// </summary>
    [GeneratedCode("ApiGenerator", "1.1.33.0")]
    public partial class WebApiStartupFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                ModifyConfiguration(config);
                var integrationConfig = new ConfigurationBuilder().Build();
                config.AddConfiguration(integrationConfig);
            });
            builder.ConfigureTestServices(services =>
            {
                ModifyServices(services);
                services.AddSingleton<RestApiOptions, RestApiOptions>();
                services.AutoRegistrateServices(Assembly.GetAssembly(typeof(ApiRegistration))!, Assembly.GetAssembly(typeof(WebApiStartupFactory))!);
            });
        }

        partial void ModifyConfiguration(IConfigurationBuilder config);

        partial void ModifyServices(IServiceCollection services);
    }
}
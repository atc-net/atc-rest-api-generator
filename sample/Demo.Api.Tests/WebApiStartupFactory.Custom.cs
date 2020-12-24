using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api.Tests
{
    /// <summary>
    /// Partial class definition that has been added to allow for customizing the generated
    /// class and its <see cref="ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder)"/>.
    /// </summary>
    public partial class WebApiStartupFactory : WebApplicationFactory<Startup>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed to show as an example.")]
        partial void ModifyConfiguration(IConfigurationBuilder config)
        {
            // Modify configuration of generated class.
            // Can be used to remove e.g. Key Vault configured in Startup.
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Needed to show as an example.")]
        partial void ModifyServices(IServiceCollection services)
        {
            // Modify services of generated class.
            // Can be used to add e.g. mocked dependencies in Startup.
        }
    }
}
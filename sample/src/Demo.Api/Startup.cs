using System.Reflection;
using Atc.Rest.Extended.Options;
using Demo.Api.Generated;
using Demo.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api
{
    public class Startup
    {
        private readonly RestApiExtendedOptions restApiOptions;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            restApiOptions = new RestApiExtendedOptions();
            restApiOptions.AddAssemblyPairs(Assembly.GetAssembly(typeof(ApiRegistration)), Assembly.GetAssembly(typeof(DomainRegistration)));
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureOptions<ConfigureSwaggerDocOptions>();

            services.AddRestApi<Startup>(restApiOptions, Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureRestApi(env, restApiOptions);
        }
    }
}
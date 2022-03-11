using System.Reflection;
using Atc;
using Atc.Rest.Extended.Options;
using Atc.Rest.Options;
using Demo.Api.Full.Configuration;
using Demo.Api.Generated;
using Demo.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.Api.Full
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            restApiOptions = new RestApiExtendedOptions
            {
                // Base
                AllowAnonymousAccessForDevelopment = true,
                UseApplicationInsights = true,
                UseAutoRegistrateServices = true,
                UseEnumAsStringInSerialization = true,
                UseHttpContextAccessor = true,
                ErrorHandlingExceptionFilter = new RestApiOptionsErrorHandlingExceptionFilter
                {
                    Enable = true,
                    UseProblemDetailsAsResponseBody = true,
                    IncludeExceptionDetails = true,
                },
                UseRequireHttpsPermanent = true,
                UseJsonSerializerOptionsIgnoreNullValues = true,
                JsonSerializerCasingStyle = CasingStyle.CamelCase,
                UseValidateServiceRegistrations = true,

                // Extended
                UseApiVersioning = true,
                UseFluentValidation = true,
                UseOpenApiSpec = true,
            };

            restApiOptions.AddAssemblyPairs(
                Assembly.GetAssembly(typeof(ApiRegistration)),
                Assembly.GetAssembly(typeof(DomainRegistration)));
        }

        public IConfiguration Configuration { get; }

        private readonly RestApiExtendedOptions restApiOptions;

        public void ConfigureServices(IServiceCollection services)
        {
            if (!restApiOptions.UseAutoRegistrateServices)
            {
                // Manual ConfigureServices
                services.ConfigureServices();
            }

            services.ConfigureOptions<ConfigureSwaggerOptions>();
            services.AddRestApi<Startup>(restApiOptions, Configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureRestApi(env, restApiOptions);
        }
    }
}
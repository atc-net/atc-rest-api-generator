using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Atc;
using Atc.Rest.Extended.Options;
using Atc.Rest.Options;
using Demo.Api.Full.Configuration;
using Demo.Api.Generated;
using Demo.Domain;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
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

            ////services.AddControllers(options =>
            ////    options.InputFormatters.Add(new ByteArrayInputFormatter()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.ConfigureRestApi(env, restApiOptions);
        }
    }

#pragma warning disable MA0048 // File name must match type name
#pragma warning disable SA1402 // File may only contain a single type
    public class ByteArrayInputFormatter : InputFormatter
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore MA0048 // File name must match type name
    {
        public ByteArrayInputFormatter()
        {
            SupportedMediaTypes.Add(Microsoft.Net.Http.Headers.MediaTypeHeaderValue.Parse("application/octet-stream"));
        }

        protected override bool CanReadType(Type type)
        {
            return type == typeof(byte[]);
        }

        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            var stream = new MemoryStream();
            await context.HttpContext.Request.Body.CopyToAsync(stream);
            ////return await InputFormatterResult.SuccessAsync(stream.ToArray());
            return await InputFormatterResult.SuccessAsync(stream.ToString());
        }
    }
}
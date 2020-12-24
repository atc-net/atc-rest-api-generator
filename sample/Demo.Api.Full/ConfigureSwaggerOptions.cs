using System;
using System.Collections.Generic;
using Atc.Rest.Extended.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Demo.Api.Full
{
     public class ConfigureSwaggerOptions :
        IConfigureOptions<SwaggerGenOptions>,
        IConfigureOptions<SwaggerUIOptions>
    {
        private readonly RestApiExtendedOptions apiOptions;
        private readonly IWebHostEnvironment env;

        public ConfigureSwaggerOptions(
            RestApiExtendedOptions options,
            IWebHostEnvironment env)
        {
            this.apiOptions = options ?? throw new ArgumentNullException(nameof(options));
            this.env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public void Configure(SwaggerGenOptions options)
        {
            if (!env.IsDevelopment())
            {
                return;
            }

            var authority = $"{apiOptions.Authorization.Instance}/{apiOptions.Authorization.TenantId}";
            var authorizationUrl = $"{authority}/oauth2/v2.0/authorize";
            var tokenUrl = $"{authority}/oauth2/v2.0/token";
            var authFlow = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri(tokenUrl),
                AuthorizationUrl = new Uri(authorizationUrl),
                Scopes = new Dictionary<string, string>
                {
                    { $"{apiOptions.Authorization.Audience}/.default", "Default" },
                },
            };

            options.AddSecurityDefinition(
                SecuritySchemeType.OAuth2.ToString(),
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = authFlow,
                        AuthorizationCode = authFlow,
                    },
                });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = SecuritySchemeType.OAuth2.ToString(),
                            },
                        },
                        Array.Empty<string>()
                    },
                });
        }

        public void Configure(SwaggerUIOptions options)
        {
            options.OAuthClientId(apiOptions.Authorization.ClientId);
            options.OAuthUsePkce();
        }
    }
}
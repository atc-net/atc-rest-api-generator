using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public class SyntaxGeneratorSwaggerDocDocOptions
    {
        private readonly string fullNamespace;
        private readonly OpenApiDocument document;

        public SyntaxGeneratorSwaggerDocDocOptions(string fullNamespace, OpenApiDocument document)
        {
            this.fullNamespace = fullNamespace;
            this.document = document;
        }

        public string GenerateCode()
        {
            var contactUrl =
                string.IsNullOrWhiteSpace(document.Info?.Contact?.Url?.ToString())
                    ? string.Empty
                    : $@"
                        Url = new Uri(""{document.Info?.Contact?.Url}""),";

            var licenseUrl =
                string.IsNullOrWhiteSpace(document.Info?.License?.Url?.ToString())
                ? string.Empty
                : $@"
                        Url = new Uri(""{document.Info?.License?.Url}""),";

            string code = $@"using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace {fullNamespace}
{{
    public class ConfigureSwaggerDocOptions : IConfigureOptions<SwaggerGenOptions>
    {{
        public void Configure(SwaggerGenOptions options)
        {{
            options.SwaggerDoc(
                ""{document.Info?.Version}"",
                new OpenApiInfo
                {{
                    Version = ""{document.Info?.Version}"",
                    Title = ""{document.Info?.Title}"",
                    Description = @""{document.Info?.Description}"",
                    Contact = new OpenApiContact()
                    {{
                        Name = ""{document.Info?.Contact?.Name}"",{contactUrl}
                        Email = ""{document.Info?.Contact?.Email}"",
                    }},
                    TermsOfService = new Uri(""{document.Info?.TermsOfService}""),
                    License = new OpenApiLicense()
                    {{
                        Name = ""{document.Info?.License?.Name}"",{licenseUrl}
                    }},
                }});
        }}
    }}
}}";
            return SyntaxFactory.ParseSyntaxTree(code)
                .GetCompilationUnitRoot()
                .ToFullString()
                .FormatAutoPropertiesOnOneLine()
                .FormatPublicPrivateLines()
                .FormatDoubleLines();
        }
    }
}
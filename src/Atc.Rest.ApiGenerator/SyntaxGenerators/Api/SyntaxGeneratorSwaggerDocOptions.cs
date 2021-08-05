using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    // TODO: FIX THIS - Use CompilationUnit
    public class SyntaxGeneratorSwaggerDocOptions
    {
        private readonly string fullNamespace;
        private readonly OpenApiDocument document;

        public SyntaxGeneratorSwaggerDocOptions(
            string fullNamespace,
            OpenApiDocument document)
        {
            this.fullNamespace = fullNamespace;
            this.document = document;
        }

        public string GenerateCode()
            => SyntaxFactory
                .ParseSyntaxTree(
                    GetSyntaxTreeText()
                        .Replace("\"\"", "null", StringComparison.OrdinalIgnoreCase))
                .GetCompilationUnitRoot()
                .ToFullString()
                .EnsureEnvironmentNewLines();

        private string GetSyntaxTreeText()
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

            var termsOfService =
                string.IsNullOrWhiteSpace(document.Info?.License?.Url?.ToString())
                    ? string.Empty
                    : $@"
                        TermsOfService = new Uri(""{document.Info?.TermsOfService}""),";

            return $@"using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace {fullNamespace}
{{
    public class ConfigureSwaggerDocOptions : IConfigureOptions<SwaggerGenOptions>
    {{
        private readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerDocOptions(IApiVersionDescriptionProvider provider)
            => this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {{
            foreach (var version in provider.ApiVersionDescriptions)
            {{
                options.SwaggerDoc(
                    version.GroupName,
                    new OpenApiInfo
                    {{
                        Version = ""{document.Info?.Version}"",
                        Title = ""{document.Info?.Title}"",
                        Description = @""{document.Info?.Description}"",
                        Contact = new OpenApiContact
                        {{
                            Name = ""{document.Info?.Contact?.Name}"",{contactUrl}
                            Email = ""{document.Info?.Contact?.Email}"",
                        }},{termsOfService}
                        License = new OpenApiLicense
                        {{
                            Name = ""{document.Info?.License?.Name}"",{licenseUrl}
                        }},
                    }});
            }}

            options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, ""xml""));
        }}
    }}
}}";
        }
    }
}
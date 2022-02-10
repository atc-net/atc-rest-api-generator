namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

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
        var version =
            string.IsNullOrWhiteSpace(document.Info?.Version)
                ? "Unknown"
                : document.Info.Version;

        var title =
            string.IsNullOrWhiteSpace(document.Info?.Title)
                ? "Unknown"
                : document.Info.Title;

        var description =
            string.IsNullOrWhiteSpace(document.Info?.Description)
                ? "Unknown"
                : document.Info.Description;

        var name =
            string.IsNullOrWhiteSpace(document.Info?.Contact?.Name)
                ? "Unknown"
                : document.Info.Contact.Name;

        var contactUrl =
            string.IsNullOrWhiteSpace(document.Info?.Contact?.Url?.ToString())
                ? string.Empty
                : $@"
                            Url = new Uri(""{document.Info?.Contact?.Url}""),";

        var email =
            string.IsNullOrWhiteSpace(document.Info?.Contact?.Email)
                ? "Unknown"
                : document.Info.Contact.Email;

        var termsOfService =
            string.IsNullOrWhiteSpace(document.Info?.License?.Url?.ToString())
                ? string.Empty
                : $@"
                        TermsOfService = new Uri(""{document.Info?.TermsOfService}""),";

        var licenseUrl =
            string.IsNullOrWhiteSpace(document.Info?.License?.Url?.ToString())
                ? string.Empty
                : $@"
                            Url = new Uri(""{document.Info?.License?.Url}""),";

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
                        Version = ""{version}"",
                        Title = ""{title}"",
                        Description = ""{description}"",
                        Contact = new OpenApiContact
                        {{
                            Name = ""{name}"",{contactUrl}
                            Email = ""{email}"",
                        }},{termsOfService}
                        License = new OpenApiLicense
                        {{
                            Name = ""{name}"",{licenseUrl}
                        }},
                    }});
            }}

            options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, ""xml""));
        }}
    }}
}}";
    }
}
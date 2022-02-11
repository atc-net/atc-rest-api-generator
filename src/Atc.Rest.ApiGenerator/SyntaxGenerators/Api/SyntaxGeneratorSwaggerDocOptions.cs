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
            .ParseSyntaxTree(GetSyntaxTreeText())
            .GetCompilationUnitRoot()
            .ToFullString()
            .EnsureEnvironmentNewLines();

    private string GetSyntaxTreeText()
    {
        var sb = new StringBuilder();

        sb.AppendLine("using System;");
        sb.AppendLine("using System.IO;");
        sb.AppendLine("using Microsoft.AspNetCore.Mvc.ApiExplorer;");
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine("using Microsoft.Extensions.Options;");
        sb.AppendLine("using Microsoft.OpenApi.Models;");
        sb.AppendLine("using Swashbuckle.AspNetCore.SwaggerGen;");
        sb.AppendLine();
        sb.AppendLine($"namespace {fullNamespace}");
        sb.AppendLine("{");
        sb.AppendLine(4, "public class ConfigureSwaggerDocOptions : IConfigureOptions<SwaggerGenOptions>");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "private readonly IApiVersionDescriptionProvider provider;");
        sb.AppendLine();
        sb.AppendLine(8, "public ConfigureSwaggerDocOptions(IApiVersionDescriptionProvider provider)");
        sb.AppendLine(12, "=> this.provider = provider;");
        sb.AppendLine();
        sb.AppendLine(8, "public void Configure(SwaggerGenOptions options)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "foreach (var version in provider.ApiVersionDescriptions)");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "options.SwaggerDoc(");
        sb.AppendLine(20, "version.GroupName,");
        sb.AppendLine(20, "new OpenApiInfo");
        sb.AppendLine(20, "{");

        if (!string.IsNullOrWhiteSpace(document.Info?.Version))
        {
            sb.AppendLine(24, $"Version = \"{document.Info.Version}\",");
        }

        if (!string.IsNullOrWhiteSpace(document.Info?.Title))
        {
            sb.AppendLine(24, $"Title = \"{document.Info.Title}\",");
        }

        if (!string.IsNullOrWhiteSpace(document.Info?.Description))
        {
            sb.AppendLine(24, $"Description = \"{document.Info.Description}\",");
        }

        if (!string.IsNullOrWhiteSpace(document.Info?.Contact?.Name) ||
            !string.IsNullOrWhiteSpace(document.Info?.Contact?.Email) ||
            !string.IsNullOrWhiteSpace(document.Info?.Contact?.Url?.ToString()))
        {
            sb.AppendLine(24, "Contact = new OpenApiContact");
            sb.AppendLine(24, "{");

            if (!string.IsNullOrWhiteSpace(document.Info?.Contact?.Name))
            {
                sb.AppendLine(28, $"Name = \"{document.Info.Contact.Name}\",");
            }

            if (!string.IsNullOrWhiteSpace(document.Info?.Contact?.Email))
            {
                sb.AppendLine(28, $"Email = \"{document.Info.Contact.Email}\",");
            }

            if (!string.IsNullOrWhiteSpace(document.Info?.Contact?.Url?.ToString()))
            {
                sb.AppendLine(28, $"Url = new Uri(\"{document.Info.Contact.Url}\"),");
            }

            sb.AppendLine(24, "},");
        }

        if (!string.IsNullOrWhiteSpace(document.Info?.TermsOfService?.ToString()))
        {
            sb.AppendLine(24, $"TermsOfService = new Uri(\"{document.Info.TermsOfService}\"),");
        }

        if (!string.IsNullOrWhiteSpace(document.Info?.License?.Name) ||
            !string.IsNullOrWhiteSpace(document.Info?.License?.Url?.ToString()))
        {
            sb.AppendLine(24, "License = new OpenApiLicense");
            sb.AppendLine(24, "{");

            if (!string.IsNullOrWhiteSpace(document.Info?.License?.Name))
            {
                sb.AppendLine(28, $"Name = \"{document.Info.License.Name}\",");
            }

            if (!string.IsNullOrWhiteSpace(document.Info?.License?.Url?.ToString()))
            {
                sb.AppendLine(28, $"Url = new Uri(\"{document.Info.License.Url}\"),");
            }

            sb.AppendLine(24, "},");
        }

        sb.AppendLine(20, "});");
        sb.AppendLine(12, "}");
        sb.AppendLine();
        sb.AppendLine(12, "options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, \"xml\"));");
        sb.AppendLine(8, "}");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
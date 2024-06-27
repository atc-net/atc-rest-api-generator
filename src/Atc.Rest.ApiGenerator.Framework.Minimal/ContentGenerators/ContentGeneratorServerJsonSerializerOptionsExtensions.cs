namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators;

public class ContentGeneratorServerJsonSerializerOptionsExtensions : IContentGenerator
{
    private readonly ContentGeneratorBaseParameters parameters;

    public ContentGeneratorServerJsonSerializerOptionsExtensions(
        ContentGeneratorBaseParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace}.Extensions;"); // TODO: Move to constant
        sb.AppendLine();
        sb.AppendLine("public static class JsonSerializerOptionsExtensions");
        sb.AppendLine("{");
        sb.AppendLine(4, "public static Microsoft.AspNetCore.Http.Json.JsonOptions Configure(");
        sb.AppendLine(8, "this System.Text.Json.JsonSerializerOptions jsonSerializerOptions,");
        sb.AppendLine(8, "Microsoft.AspNetCore.Http.Json.JsonOptions options)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(jsonSerializerOptions);");
        sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(options);");
        sb.AppendLine();
        sb.AppendLine(8, "options.SerializerOptions.DefaultIgnoreCondition = jsonSerializerOptions.DefaultIgnoreCondition;");
        sb.AppendLine(8, "options.SerializerOptions.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;");
        sb.AppendLine(8, "options.SerializerOptions.WriteIndented = jsonSerializerOptions.WriteIndented;");
        sb.AppendLine(8, "options.SerializerOptions.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;");
        sb.AppendLine();
        sb.AppendLine(8, "foreach (var jsonConverter in jsonSerializerOptions.Converters)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "options.SerializerOptions.Converters.Add(jsonConverter);");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "return options;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public static Microsoft.AspNetCore.Mvc.JsonOptions Configure(");
        sb.AppendLine(8, "this System.Text.Json.JsonSerializerOptions jsonSerializerOptions,");
        sb.AppendLine(8, "Microsoft.AspNetCore.Mvc.JsonOptions options)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(jsonSerializerOptions);");
        sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(options);");
        sb.AppendLine();
        sb.AppendLine(8, "options.JsonSerializerOptions.DefaultIgnoreCondition = jsonSerializerOptions.DefaultIgnoreCondition;");
        sb.AppendLine(8, "options.JsonSerializerOptions.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;");
        sb.AppendLine(8, "options.JsonSerializerOptions.WriteIndented = jsonSerializerOptions.WriteIndented;");
        sb.AppendLine(8, "options.JsonSerializerOptions.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;");
        sb.AppendLine();
        sb.AppendLine(8, "foreach (var jsonConverter in jsonSerializerOptions.Converters)");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "options.JsonSerializerOptions.Converters.Add(jsonConverter);");
        sb.AppendLine(8, "}");
        sb.AppendLine();
        sb.AppendLine(8, "return options;");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }
}
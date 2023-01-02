namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerHandler : IContentGenerator
{
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerHandlerParameters parameters;

    public ContentGeneratorServerHandler(
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerHandlerParameters parameters)
    {
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        }

        sb.AppendLine($"public class {parameters.HandlerName} : {parameters.InterfaceName}");
        sb.AppendLine("{");

        sb.AppendLine(4, $"public Task<{parameters.ResultName}> ExecuteAsync(");
        if (parameters.ParameterName is not null)
        {
            sb.AppendLine(8, $"{parameters.ParameterName} parameters,");
        }

        sb.AppendLine(8, "CancellationToken cancellationToken = default)");
        sb.AppendLine(4, "{");
        if (parameters.ParameterName is not null)
        {
            sb.AppendLine(8, "ArgumentNullException.ThrowIfNull(parameters);");
            sb.AppendLine();
        }

        sb.AppendLine(8, $"throw new NotImplementedException(\"Add logic here for {parameters.HandlerName}\");");
        sb.AppendLine(4, "}");

        sb.AppendLine("}");

        return sb.ToString();
    }
}
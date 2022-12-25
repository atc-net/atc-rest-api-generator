namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerHandlerInterface : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerHandlerInterfaceParameters parameters;

    public ContentGeneratorServerHandlerInterface(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerHandlerInterfaceParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public interface {parameters.InterfaceName}");
        sb.AppendLine("{");
        sb.Append(codeDocumentationTagsGenerator.GenerateHandlerMethodTags(4, parameters.ParameterName));
        sb.AppendLine(4, $"Task<{parameters.ResultName}> ExecuteAsync(");
        if (!string.IsNullOrEmpty(parameters.ParameterName))
        {
            sb.AppendLine(8, $"{parameters.ParameterName} parameters,");
        }

        sb.AppendLine(8, "CancellationToken cancellationToken = default);");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Client;

public class ContentGeneratorClientEndpointResultInterface : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorClientEndpointResultInterfaceParameters parameters;

    public ContentGeneratorClientEndpointResultInterface(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorClientEndpointResultInterfaceParameters parameters)
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
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTagsForClass))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTagsForClass));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public interface {parameters.InterfaceName} : {parameters.InheritInterfaceName}");
        sb.AppendLine("{");
        sb.AppendLine(4, "bool IsOk { get; }");
        foreach (var item in parameters.ErrorResponses)
        {
            sb.AppendLine();
            sb.AppendLine(4, $"bool Is{item.StatusCode.ToNormalizedString()} {{ get; }}");
        }

        sb.AppendLine();
        sb.AppendLine(
            4,
            parameters.UseListForModel
                ? $"List<{parameters.SuccessResponseName}> OkContent {{ get; }}"
                : $"{parameters.SuccessResponseName} OkContent {{ get; }}");

        foreach (var item in parameters.ErrorResponses)
        {
            if (item.StatusCode == HttpStatusCode.BadRequest)
            {
                sb.AppendLine();
                sb.AppendLine(4, $"ValidationProblemDetails {item.StatusCode.ToNormalizedString()}Content {{ get; }}");
            }
            else
            {
                if (item.StatusCode == HttpStatusCode.NotModified)
                {
                    // NotModified does not return any content to the client
                    continue;
                }

                if (parameters.UseProblemDetailsAsDefaultBody)
                {
                    sb.AppendLine();
                    sb.AppendLine(4, $"ProblemDetails {item.StatusCode.ToNormalizedString()}Content {{ get; }}");
                }
                else
                {
                    if (string.IsNullOrEmpty(item.ResponseType))
                    {
                        // Skip
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.AppendLine(4, $"{item.ResponseType} {item.StatusCode.ToNormalizedString()}Content {{ get; }}");
                    }
                }
            }
        }

        sb.Append('}');

        return sb.ToString();
    }
}
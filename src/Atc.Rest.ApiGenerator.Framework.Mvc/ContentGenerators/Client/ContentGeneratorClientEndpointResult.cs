// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Client;

public class ContentGeneratorClientEndpointResult : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorClientEndpointResultParameters parameters;

    public ContentGeneratorClientEndpointResult(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorClientEndpointResultParameters parameters)
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
        sb.AppendLine($"public class {parameters.EndpointResultName} : {parameters.InheritClassName}");
        sb.AppendLine("{");
        sb.AppendLine(4, $"public {parameters.EndpointResultName}(EndpointResponse response)");
        sb.AppendLine(4, ": base(response)");
        sb.AppendLine(4, "{");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public bool IsOk");
        sb.AppendLine(8, "=> StatusCode == HttpStatusCode.OK;");
        foreach (var item in parameters.ErrorResponses)
        {
            sb.AppendLine();
            sb.AppendLine(4, $"public bool Is{item.StatusCode.ToNormalizedString()}");
            sb.AppendLine(8, $"=> StatusCode == HttpStatusCode.{item.StatusCode};");
        }

        sb.AppendLine(
            4,
            parameters.UseListForModel
                ? $"public List<{parameters.SuccessResponseName}> OkContent"
                : $"public {parameters.SuccessResponseName} OkContent");
        sb.AppendLine(
            8,
            parameters.UseListForModel
                ? $"=> IsOk && ContentObject is List<{parameters.SuccessResponseName}> result"
                : $"=> IsOk && ContentObject is {parameters.SuccessResponseName} result");
        sb.AppendLine(12, "? result");
        sb.AppendLine(12, ": throw new InvalidOperationException(\"Content is not the expected type - please use the IsOk property first.\");");
        foreach (var item in parameters.ErrorResponses)
        {
            // TODO: Fix If->ProblemDetails
            sb.AppendLine();
            sb.AppendLine(4, $"public ProblemDetails {item.StatusCode.ToNormalizedString()}Content");
            sb.AppendLine(8, $"=> Is{item.StatusCode.ToNormalizedString()} && ContentObject is ProblemDetails result");
            sb.AppendLine(12, "? result");
            sb.AppendLine(12, $": throw new InvalidOperationException(\"Content is not the expected type - please use the Is{item.StatusCode.ToNormalizedString()} property first.\");");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.ServerClient;

public sealed class ContentGeneratorServerClientEnumeration : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerClientEnumerationParameters parameters;

    public ContentGeneratorServerClientEnumeration(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerClientEnumerationParameters parameters)
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
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());
        if (parameters.UseFlagsAttribute)
        {
            sb.AppendLine("[Flags]");
        }

        sb.AppendLine($"public enum {parameters.EnumerationName}");
        sb.AppendLine("{");

        foreach (var valueParameter in parameters.ValueParameters)
        {
            sb.AppendLine(
                4,
                valueParameter.Value is null
                    ? $"{valueParameter.Name},"
                    : $"{valueParameter.Name} = {valueParameter.Value},");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}
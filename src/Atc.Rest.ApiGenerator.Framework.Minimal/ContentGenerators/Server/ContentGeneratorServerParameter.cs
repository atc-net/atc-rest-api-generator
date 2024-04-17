namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators.Server;

public sealed class ContentGeneratorServerParameter : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerParameterParameters parameters;

    public ContentGeneratorServerParameter(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerParameterParameters parameters)
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
        sb.Append($"public record {parameters.ParameterName}");

        if (!parameters.PropertyParameters.Any())
        {
            sb.Append("();");
        }
        else
        {
            sb.AppendLine("(");
            const int indentSpaces = 4;

            for (var j = 0; j < parameters.PropertyParameters.Count; j++)
            {
                var item = parameters.PropertyParameters[j];
                var useCommaForEndChar = j != parameters.PropertyParameters.Count - 1;
                sb.AppendInputParameter(
                    indentSpaces,
                    usePropertyPrefix: true,
                    attributes: ExtractAttributes(item),
                    genericTypeName: null,
                    typeName: item.DataType,
                    item.IsNullable,
                    item.ParameterName,
                    item.DefaultValueInitializer,
                    useCommaForEndChar);
            }

            sb.Append(';');
        }

        return sb.ToString();
    }

    private static IList<AttributeParameters> ExtractAttributes(
        ContentGeneratorServerParameterParametersProperty item)
    {
        var result = new List<AttributeParameters>();

        if (item.ParameterLocationType != ParameterLocationType.None)
        {
            result.Add(AttributeParametersFactory.Create($"From{item.ParameterLocationType}"));
        }

        if (item.IsRequired)
        {
            result.Add(AttributeParametersFactory.Create("Required"));
        }

        result.AddRange(AttributesParametersFactory.Create(item.AdditionalValidationAttributes));

        return result;
    }
}
// ReSharper disable MergeIntoPattern
namespace Atc.CodeGeneration.CSharp.Content.Generators;

[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "OK.")]
public class GenerateContentForEnum : IContentGenerator
{
    private readonly ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly EnumParameters parameters;

    public GenerateContentForEnum(
        ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        EnumParameters parameters)
    {
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var contentWriter = new GenerateContentWriter(codeDocumentationTagsGenerator);

        var useJsonStringEnumConverter = UseJsonStringEnumConverter(parameters.Values);

        var sb = new StringBuilder();
        sb.Append(
            contentWriter.GenerateTopOfType(
                parameters.HeaderContent,
                parameters.Namespace,
                parameters.DocumentationTags,
                GetAttributeParametersList()));

        if (useJsonStringEnumConverter)
        {
            sb.AppendLine("[JsonConverter(typeof(JsonStringEnumConverter))]");
        }

        sb.Append($"{parameters.DeclarationModifier.GetDescription()} enum ");
        sb.AppendLine($"{parameters.EnumTypeName}");

        sb.AppendLine("{");
        foreach (var parametersValue in parameters.Values)
        {
            if (parametersValue.DescriptionAttribute is not null)
            {
                sb.AppendAttribute(4, usePropertyPrefix: false, parametersValue.DescriptionAttribute);
                sb.AppendLine();
            }

            var sbLine = new StringBuilder();

            if (useJsonStringEnumConverter)
            {
                if ("*".Equals(parametersValue.Name, StringComparison.Ordinal))
                {
                    sbLine.AppendLine("[EnumMember(Value = \"*\")]");
                    sbLine.Append(4, "None");
                }
                else
                {
                    sbLine.AppendLine($"[EnumMember(Value = \"{parametersValue.Name}\")]");
                    sbLine.Append(4, parametersValue.Name.PascalCase(removeSeparators: true));
                }
            }
            else
            {
                sbLine.Append(parametersValue.Name);
            }

            if (parametersValue.Value is not null)
            {
                sbLine.Append($" = {parametersValue.Value}");
            }

            sbLine.Append(',');
            sb.AppendLine(4, sbLine.ToString());

            if (useJsonStringEnumConverter &&
                parameters.Values.Last() != parametersValue)
            {
                sb.AppendLine();
            }
        }

        sb.Append('}');

        return sb.ToString();
    }

    private IList<AttributeParameters>? GetAttributeParametersList()
    {
        var attributeParameters = parameters.Attributes?.ToList();
        if (parameters.UseFlags &&
            attributeParameters is not null &&
            attributeParameters.Find(x => x.Name == "Flags") is null)
        {
            attributeParameters.Add(
                new AttributeParameters(
                    Name: "Flags",
                    Content: null));
        }

        return attributeParameters;
    }

    private static bool UseJsonStringEnumConverter(
        IList<EnumValueParameters> parametersValues)
        => parametersValues.Any(x => x.Name.IsFirstCharacterLowerCase() ||
                                     x.Name.Contains('-', StringComparison.Ordinal));
}
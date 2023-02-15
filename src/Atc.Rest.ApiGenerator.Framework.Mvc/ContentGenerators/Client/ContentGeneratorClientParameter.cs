// ReSharper disable StringLiteralTypo
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Client;

public sealed class ContentGeneratorClientParameter : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorClientParameterParameters parameters;

    public ContentGeneratorClientParameter(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorClientParameterParameters parameters)
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
        sb.AppendLine($"public class {parameters.ParameterName}");
        sb.AppendLine("{");

        foreach (var parameter in parameters.PropertyParameters)
        {
            if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameter.DocumentationTags))
            {
                sb.Append(codeDocumentationTagsGenerator.GenerateTags(4, parameter.DocumentationTags));
            }

            AppendPropertyAttributes(sb, parameter);
            AppendPropertyBody(sb, parameter);

            sb.AppendLine();
        }

        AppendMethodToStringContent(sb, parameters.PropertyParameters);

        sb.Append('}');

        return sb.ToString();
    }

    private static void AppendPropertyAttributes(
        StringBuilder sb,
        ContentGeneratorClientParameterParametersProperty item)
    {
        switch (item.ParameterLocationType)
        {
            case ParameterLocationType.Query:
            case ParameterLocationType.Header:
            case ParameterLocationType.Route:
            case ParameterLocationType.Cookie:
                sb.AppendLine(4, $"[From{item.ParameterLocationType}(Name = \"{item.Name}\")]");
                break;
            case ParameterLocationType.Body:
            case ParameterLocationType.Form:
                sb.AppendLine(4, $"[From{item.ParameterLocationType}]");
                break;
        }

        if (item.IsRequired)
        {
            sb.AppendLine(4, "[Required]");
        }

        foreach (var additionalValidationAttribute in item.AdditionalValidationAttributes)
        {
            switch (additionalValidationAttribute)
            {
                case EmailAddressAttribute:
                    sb.AppendLine(4, "[EmailAddress]");
                    break;
                case RegularExpressionAttribute regularExpressionAttribute:
                    sb.AppendLine(4, $"[RegularExpression(\"{regularExpressionAttribute.Pattern}\")]");
                    break;
                case StringLengthAttribute stringLengthAttribute:
                    sb.AppendLine(4, $"[StringLength({stringLengthAttribute.MaximumLength})]");
                    break;
                case MinLengthAttribute minLengthAttribute:
                    sb.AppendLine(4, $"[MinLength({minLengthAttribute.Length})]");
                    break;
                case MaxLengthAttribute maxLengthAttribute:
                    sb.AppendLine(4, $"[MaxLength({maxLengthAttribute.Length})]");
                    break;
                case RangeAttribute rangeAttribute:
                    sb.AppendLine(4, $"[Range({rangeAttribute.Minimum}, {rangeAttribute.Maximum})]");
                    break;
                case UriAttribute:
                    sb.AppendLine(4, "[Uri]");
                    break;
            }
        }
    }

    private static void AppendPropertyBody(
        StringBuilder sb,
        ContentGeneratorClientParameterParametersProperty item)
    {
        sb.Append(4, "public ");
        if (item.UseListForDataType)
        {
            sb.Append("List<");
        }

        sb.Append(item.DataType);
        if (item.UseListForDataType)
        {
            sb.Append('>');
        }

        if (item.IsNullable)
        {
            sb.Append('?');
        }

        sb.Append(' ');
        sb.Append(item.ParameterName);

        if (item.UseListForDataType)
        {
            sb.AppendLine($" {{ get; set; }} = new List<{item.DataType}>();");
        }
        else
        {
            if (string.IsNullOrEmpty(item.DefaultValueInitializer))
            {
                sb.AppendLine(" { get; set; }");
            }
            else
            {
                sb.AppendLine($" {{ get; set; }} = {item.DefaultValueInitializer};");
            }
        }
    }

    private static void AppendMethodToStringContent(
        StringBuilder sb,
        IList<ContentGeneratorClientParameterParametersProperty> items)
    {
        var sbToStringContent = new StringBuilder();
        for (var i = 0; i < items.Count; i++)
        {
            var parameter = items[i];
            var parameterName = parameter.ParameterName;
            if (parameter.UseListForDataType)
            {
                sbToStringContent.Append($"{{nameof({parameterName})}}.Count: {{{parameterName}?.Count ?? 0}}");
            }
            else
            {
                if (parameter.IsSimpleType)
                {
                    sbToStringContent.Append($"{{nameof({parameterName})}}: {{{parameterName}}}");
                }
                else
                {
                    sbToStringContent.Append($"{{nameof({parameterName})}}: ({{{parameterName}}})");
                }
            }

            if (i != items.Count - 1)
            {
                sbToStringContent.Append(", ");
            }
        }

        sb.AppendLine(4, "/// <inheritdoc />");
        sb.AppendLine(4, "public override string ToString()");
        sb.AppendLine(8, $"=> $\"{sbToStringContent}\";");
    }
}
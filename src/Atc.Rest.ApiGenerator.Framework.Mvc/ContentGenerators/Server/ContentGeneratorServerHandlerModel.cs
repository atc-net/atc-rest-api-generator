// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable StringLiteralTypo
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerHandlerModel : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerHandlerModelParameters parameters;

    public ContentGeneratorServerHandlerModel(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerHandlerModelParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        AppendClassSummery(sb, parameters.Description);
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.ModelName}");
        sb.AppendLine("{");

        foreach (var parameter in parameters.PropertyParameters)
        {
            AppendPropertySummery(sb, parameter.Description);
            AppendPropertyAttributes(sb, parameter);
            AppendPropertyBody(sb, parameter);

            sb.AppendLine();
        }

        AppendMethodToStringContent(sb, parameters.PropertyParameters);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void AppendClassSummery(
        StringBuilder sb,
        string description)
    {
        if (string.IsNullOrEmpty(description))
        {
            return;
        }

        sb.AppendLine("/// <summary>");
        if (description.EndsWith('.'))
        {
            sb.AppendLine($"/// {description}");
        }
        else
        {
            sb.AppendLine($"/// {description}.");
        }

        sb.AppendLine("/// </summary>");
    }

    private static void AppendPropertySummery(
        StringBuilder sb,
        string description)
    {
        if (string.IsNullOrEmpty(description) ||
            ContentGeneratorConstants.UndefinedDescription.Equals(description, StringComparison.Ordinal))
        {
            return;
        }

        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// {description.Replace("\n", "\n/// ", StringComparison.Ordinal)}");
        sb.AppendLine(4, "/// </summary>");
    }

    private static void AppendPropertyAttributes(
        StringBuilder sb,
        ContentGeneratorServerHandlerModelParametersProperty item)
    {
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
        ContentGeneratorServerHandlerModelParametersProperty item)
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

        if (string.IsNullOrEmpty(item.DefaultValueInitializer))
        {
            sb.AppendLine(" { get; set; }");
        }
        else
        {
            sb.AppendLine($" {{ get; set; }} = {item.DefaultValueInitializer};");
        }
    }

    private static void AppendMethodToStringContent(
        StringBuilder sb,
        IList<ContentGeneratorServerHandlerModelParametersProperty> items)
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
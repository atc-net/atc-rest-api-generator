// ReSharper disable StringLiteralTypo
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerHandlerParameter : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerHandlerParameterParameters parameters;

    public ContentGeneratorServerHandlerParameter(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerHandlerParameterParameters parameters)
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
        AppendClassSummery(sb, parameters);
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.ParameterName}");
        sb.AppendLine("{");

        foreach (var parameter in parameters.Parameters)
        {
            AppendPropertyContent(sb, parameter);

            sb.AppendLine();
        }

        AppendMethodToStringContent(sb, parameters.Parameters);

        sb.AppendLine("}");

        return sb.ToString();
    }

    private static void AppendClassSummery(
        StringBuilder sb,
        ContentGeneratorServerHandlerParameterParameters item)
    {
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Parameters for operation request.");
        sb.AppendLine($"/// Description: {item.Description}");
        sb.AppendLine($"/// Operation: {item.OperationName}.");
        sb.AppendLine("/// </summary>");
    }

    private static void AppendPropertySummery(
        StringBuilder sb,
        string description)
    {
        if (ContentGeneratorConstants.UndefinedDescription.Equals(description, StringComparison.Ordinal))
        {
            return;
        }

        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// {description}");
        sb.AppendLine(4, "/// </summary>");
    }

    private static void AppendPropertyContent(
        StringBuilder sb,
        ContentGeneratorServerParameterParametersProperty item)
    {
        AppendPropertySummery(sb, item.Description);

        switch (item.ParameterLocationType)
        {
            case ParameterLocationType.Query:
            case ParameterLocationType.Header:
            case ParameterLocationType.Route:
            case ParameterLocationType.Cookie:
                sb.AppendLine(4, $"[From{item.ParameterLocationType}(Name = \"{item.Name}\")]");
                break;
            case ParameterLocationType.Body:
                sb.AppendLine(4, $"[From{item.ParameterLocationType}]");
                break;
        }

        if (item.IsRequired)
        {
            sb.AppendLine(4, "[Required]");
        }

        sb.Append(4, "public ");
        sb.Append(item.DataType);
        if (item.IsNullable)
        {
            sb.Append('?');
        }

        sb.Append(' ');
        sb.Append(item.ParameterName);
        sb.AppendLine(" { get; set; }");
    }

    private static void AppendMethodToStringContent(
        StringBuilder sb,
        IReadOnlyList<ContentGeneratorServerParameterParametersProperty> items)
    {
        var sbToStringContent = new StringBuilder();
        for (var i = 0; i < items.Count; i++)
        {
            var parameterName = items[i].ParameterName;
            sbToStringContent.Append($"{{nameof({parameterName})}}: {{{parameterName}}}");
            if (i != items.Count - 1)
            {
                sbToStringContent.Append(", ");
            }
        }

        sb.AppendLine(4, "/// <inheritdoc />");
        sb.AppendLine(4, "public override string ToString()");
        sb.AppendLine(7, $"=> $\"{sbToStringContent}\";");
    }
}
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
namespace Atc.CodeGeneration.CSharp.Content.Generators;

public class GenerateContentForInterface : IContentGenerator
{
    private readonly ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly InterfaceParameters parameters;

    public GenerateContentForInterface(
        ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        InterfaceParameters parameters)
    {
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var contentWriter = new GenerateContentWriter(codeDocumentationTagsGenerator);

        var sb = new StringBuilder();
        sb.Append(
            contentWriter.GenerateTopOfType(
                parameters.HeaderContent,
                parameters.Namespace,
                parameters.DocumentationTags,
                parameters.Attributes));

        sb.Append($"{parameters.AccessModifier.GetDescription()} ");
        if (string.IsNullOrEmpty(parameters.InheritedInterfaceTypeName))
        {
            sb.AppendLine($"{parameters.InterfaceTypeName}");
        }
        else
        {
            sb.AppendLine($"{parameters.InterfaceTypeName} : {parameters.InheritedInterfaceTypeName}");
        }

        sb.AppendLine("{");

        var isFirstEntry = true;
        if (parameters.Properties is not null)
        {
            foreach (var propertyParameters in parameters.Properties)
            {
                if (!isFirstEntry)
                {
                    sb.AppendLine();
                }

                sb.AppendLine(contentWriter.GenerateProperty(propertyParameters));

                isFirstEntry = false;
            }
        }

        if (parameters.Methods is not null)
        {
            foreach (var methodParameters in parameters.Methods)
            {
                if (!isFirstEntry)
                {
                    sb.AppendLine();
                }

                sb.AppendLine(contentWriter.GenerateMethod(methodParameters));

                isFirstEntry = false;
            }
        }

        sb.Append('}');

        return sb.ToString();
    }
}
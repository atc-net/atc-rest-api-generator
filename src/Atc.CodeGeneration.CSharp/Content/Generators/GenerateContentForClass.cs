// ReSharper disable MergeIntoPattern
namespace Atc.CodeGeneration.CSharp.Content.Generators;

public class GenerateContentForClass : IContentGenerator
{
    private readonly ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ClassParameters parameters;

    public GenerateContentForClass(
        ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ClassParameters parameters)
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

        sb.Append($"{parameters.AccessModifier.GetDescription()} class ");
        if (string.IsNullOrEmpty(parameters.InheritedClassTypeName) &&
            string.IsNullOrEmpty(parameters.InheritedInterfaceTypeName))
        {
            sb.AppendLine($"{parameters.ClassTypeName}");
        }
        else
        {
            if (!string.IsNullOrEmpty(parameters.InheritedClassTypeName) &&
                string.IsNullOrEmpty(parameters.InheritedInterfaceTypeName))
            {
                sb.AppendLine($"{parameters.ClassTypeName} : {parameters.InheritedClassTypeName}");
            }
            else if (string.IsNullOrEmpty(parameters.InheritedClassTypeName) &&
                     !string.IsNullOrEmpty(parameters.InheritedInterfaceTypeName))
            {
                sb.AppendLine($"{parameters.ClassTypeName} : {parameters.InheritedInterfaceTypeName}");
            }
            else if (!string.IsNullOrEmpty(parameters.InheritedClassTypeName) &&
                     !string.IsNullOrEmpty(parameters.InheritedInterfaceTypeName))
            {
                sb.AppendLine($"{parameters.ClassTypeName} : {parameters.InheritedClassTypeName}, {parameters.InheritedInterfaceTypeName}");
            }
        }

        sb.AppendLine("{");

        var isFirstEntry = true;
        if (parameters.Constructors is not null)
        {
            var content = contentWriter.GeneratePrivateReadonlyMembersToConstructor(parameters.Constructors);
            if (!string.IsNullOrEmpty(content))
            {
                sb.AppendLine(content);
            }

            foreach (var constructorParameters in parameters.Constructors)
            {
                if (!isFirstEntry)
                {
                    sb.AppendLine();
                }

                sb.AppendLine(contentWriter.GenerateConstructor(constructorParameters));

                isFirstEntry = false;
            }
        }

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

                sb.AppendLine(contentWriter.GenerateMethode(methodParameters));

                isFirstEntry = false;
            }
        }

        if (parameters.GenerateToStringMethode &&
            parameters.Properties is not null)
        {
            if (!isFirstEntry)
            {
                sb.AppendLine();
            }

            sb.AppendLine(contentWriter.GenerateMethodeToString(parameters.Properties));
        }

        sb.Append('}');

        return sb.ToString();
    }
}
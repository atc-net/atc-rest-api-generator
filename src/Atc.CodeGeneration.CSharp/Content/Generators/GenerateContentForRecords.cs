// ReSharper disable ForCanBeConvertedToForeach
namespace Atc.CodeGeneration.CSharp.Content.Generators;

public class GenerateContentForRecords : IContentGenerator
{
    private readonly ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly RecordsParameters parameters;

    public GenerateContentForRecords(
        ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        RecordsParameters parameters)
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

        for (var i = 0; i < parameters.Parameters.Count; i++)
        {
            var recordParameters = parameters.Parameters[i];

            sb.Append($"{recordParameters.AccessModifier.GetDescription()} {recordParameters.Name}");
            if (recordParameters.Parameters is null ||
                !recordParameters.Parameters.Any())
            {
                sb.Append("();");
            }
            else
            {
                var indentSpaces = 0;
                if (parameters.Parameters.Count == 1)
                {
                    sb.Append('(');
                }
                else
                {
                    sb.AppendLine("(");
                    indentSpaces = 4;
                }

                for (var j = 0; j < recordParameters.Parameters.Count; j++)
                {
                    var item = recordParameters.Parameters[j];
                    var useCommaForEndChar = j != recordParameters.Parameters.Count - 1;
                    sb.AppendInputParameter(
                        indentSpaces,
                        item.Attributes,
                        item.GenericTypeName,
                        item.TypeName,
                        item.Name,
                        item.DefaultValue,
                        useCommaForEndChar);
                }
            }

            sb.Append(';');

            if (i != parameters.Parameters.Count - 1)
            {
                sb.AppendLine();
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}
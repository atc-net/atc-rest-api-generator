// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable InvertIf
namespace Atc.CodeGeneration.CSharp.Content.Generators.Internal;

public class GenerateContentWriter
{
    private readonly ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator;

    public GenerateContentWriter(
        ICodeDocumentationTagsGenerator codeDocumentationTagsGenerator)
    {
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
    }

    public string GenerateTopOfType(
        string? headerContent,
        string @namespace,
        CodeDocumentationTags? documentationTags,
        IList<AttributeParameters>? attributes)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(headerContent))
        {
            sb.Append(headerContent);
        }

        sb.AppendLine($"namespace {@namespace};");
        sb.AppendLine();
        if (documentationTags is not null)
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, documentationTags));
        }

        if (attributes is not null)
        {
            foreach (var item in attributes)
            {
                if (string.IsNullOrEmpty(item.Content))
                {
                    sb.AppendLine($"[{item.Name}]");
                }
                else
                {
                    sb.AppendLine($"[{item.Name}({item.Content})]");
                }
            }
        }

        return sb.ToString();
    }

    public string GenerateProperty(
        PropertyParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        if (parameters.DocumentationTags is not null)
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(4, parameters.DocumentationTags));
        }

        if (parameters.Attributes is not null)
        {
            foreach (var item in parameters.Attributes)
            {
                if (string.IsNullOrEmpty(item.Content))
                {
                    sb.AppendLine($"[{item.Name}]");
                }
                else
                {
                    sb.AppendLine($"[{item.Name}({item.Content})]");
                }
            }
        }

        sb.Append("    ");
        if (parameters.AccessModifier != AccessModifiers.None)
        {
            sb.Append($"{parameters.AccessModifier.ToStringLowerCase()} ");
        }

        if (string.IsNullOrEmpty(parameters.ReturnGenericTypeName))
        {
            sb.Append($"{parameters.ReturnTypeName} ");
        }
        else
        {
            sb.Append($"{parameters.ReturnGenericTypeName}<{parameters.ReturnTypeName}> ");
        }

        sb.Append(parameters.Name);

        if (parameters.UseAutoProperty)
        {
            sb.Append(" { ");
            if (parameters.UseGet)
            {
                sb.Append("get; ");
            }

            if (parameters.UseSet)
            {
                sb.Append("set; ");
            }

            sb.Append('}');

            if (parameters.UseExpressionBody &&
               !string.IsNullOrEmpty(parameters.Content))
            {
                sb.AppendLine($" => {parameters.Content};");
            }
        }
        else if (!string.IsNullOrEmpty(parameters.Content))
        {
            sb.AppendLine();
            if (parameters.UseExpressionBody)
            {
                sb.AppendLine(8, $"=> {parameters.Content}");
            }
            else
            {
                var lines = parameters.Content
                    .EnsureEnvironmentNewLines()
                    .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    sb.AppendLine(8, line);
                }
            }
        }
        else
        {
            throw new ParametersException("Content is missing or use UseAutoProperty");
        }

        return sb.ToString();
    }

    public string GenerateMethode(
        MethodParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        if (parameters.DocumentationTags is not null)
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(4, parameters.DocumentationTags));
        }

        if (parameters.Attributes is not null)
        {
            foreach (var item in parameters.Attributes)
            {
                if (string.IsNullOrEmpty(item.Content))
                {
                    sb.AppendLine($"[{item.Name}]");
                }
                else
                {
                    sb.AppendLine($"[{item.Name}({item.Content})]");
                }
            }
        }

        sb.Append("    ");
        if (parameters.AccessModifier != AccessModifiers.None)
        {
            sb.Append($"{parameters.AccessModifier.ToStringLowerCase()} ");
        }

        if (string.IsNullOrEmpty(parameters.ReturnGenericTypeName))
        {
            sb.Append($"{parameters.ReturnTypeName} ");
        }
        else
        {
            sb.Append($"{parameters.ReturnGenericTypeName}<{parameters.ReturnTypeName}> ");
        }

        sb.Append(parameters.Name);
        if (parameters.Parameters is not null &&
            parameters.Parameters.Any())
        {
            sb.AppendLine("(");
            for (var i = 0; i < parameters.Parameters.Count; i++)
            {
                var item = parameters.Parameters[i];
                if (i == parameters.Parameters.Count - 1)
                {
                    if (string.IsNullOrEmpty(item.ReturnGenericTypeName))
                    {
                        sb.AppendLine(8, $"{item.ReturnGenericTypeName}<{item.ReturnTypeName}> {item.Name})");
                    }
                    else
                    {
                        sb.AppendLine(8, $"{item.ReturnTypeName} {item.Name})");
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(item.ReturnGenericTypeName))
                    {
                        sb.AppendLine(8, $"{item.ReturnGenericTypeName}<{item.ReturnTypeName}> {item.Name},");
                    }
                    else
                    {
                        sb.AppendLine(8, $"{item.ReturnTypeName} {item.Name},");
                    }
                }

                if (string.IsNullOrEmpty(parameters.Content))
                {
                    sb.AppendLine(4, "{");
                    sb.AppendLine(4, "}");
                }
                else
                {
                    sb.Append(parameters.Content);
                }
            }
        }
        else
        {
            if (string.IsNullOrEmpty(parameters.Content))
            {
                sb.AppendLine("();");
            }
            else
            {
                sb.AppendLine("()");
                sb.Append(parameters.Content);
            }
        }

        return sb.ToString();
    }
}
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable InvertIf
// ReSharper disable StringLiteralTypo
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
            sb.AppendLine(headerContent);
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

    public string GenerateConstructor(
        ConstructorParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        sb.Append(4, $"{parameters.AccessModifier.GetDescription()} ");
        if (string.IsNullOrEmpty(parameters.GenericTypeName))
        {
            sb.Append($"{parameters.TypeName}(");
        }
        else
        {
            sb.Append($"{parameters.GenericTypeName}<{parameters.TypeName}>(");
        }

        if (parameters.Parameters is not null)
        {
            if (parameters.Parameters.Count(x => x.PassToInheritedClass) == 1)
            {
                var firstParameterParameters = parameters.Parameters.First();
                if (firstParameterParameters.CreateAaOneLiner)
                {
                    sb.Append($"{firstParameterParameters.TypeName} {firstParameterParameters.Name})");
                }
                else
                {
                    sb.AppendLine($"{firstParameterParameters.TypeName} {firstParameterParameters.Name})");
                }
            }
            else
            {
                sb.AppendLine();
                for (var i = 0; i < parameters.Parameters.Count; i++)
                {
                    var item = parameters.Parameters[i];
                    var useCommaForEndChar = i != parameters.Parameters.Count - 1;
                    AppendInputParameter(
                        sb,
                        8,
                        attributes: null,
                        item.GenericTypeName,
                        item.TypeName,
                        item.Name,
                        item.DefaultValue,
                        useCommaForEndChar);
                }

                sb.AppendLine();
                sb.AppendLine(4, "{");
                foreach (var item in parameters.Parameters)
                {
                    sb.AppendLine(8, $"this.{item.Name} = {item.Name};");
                }

                sb.Append(4, "}");
            }
        }

        if (!string.IsNullOrEmpty(parameters.InheritedClassTypeName))
        {
            if (parameters.Parameters is not null &&
                parameters.Parameters.Count(x => x.PassToInheritedClass) == 1)
            {
                var firstParameterParameters = parameters.Parameters.First();
                if (firstParameterParameters.CreateAaOneLiner)
                {
                    sb.Append($" : {parameters.InheritedClassTypeName}({firstParameterParameters.Name}) {{ }}");
                }
                else
                {
                    sb.AppendLine(8, $": {parameters.InheritedClassTypeName}({firstParameterParameters.Name})");
                    sb.AppendLine(4, "{");
                    sb.Append("    }");
                }
            }
            else
            {
                // TODO Handle this.
            }
        }

        return sb.ToString();
    }

    public string? GeneratePrivateReadonlyMembersToConstructor(
        IList<ConstructorParameters> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        foreach (var parametersConstructor in parameters)
        {
            if (parametersConstructor.Parameters is not null)
            {
                foreach (var parametersConstructorParameter in parametersConstructor.Parameters)
                {
                    if (parametersConstructorParameter.CreateAsPrivateReadonlyMember)
                    {
                        sb.AppendLine(4, $"private readonly {parametersConstructorParameter.TypeName} {parametersConstructorParameter.Name};");
                    }
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
                    sb.AppendLine(4, $"[{item.Name}]");
                }
                else
                {
                    sb.AppendLine(4, $"[{item.Name}({item.Content})]");
                }
            }
        }

        sb.Append("    ");
        if (parameters.AccessModifier != AccessModifiers.None)
        {
            sb.Append($"{parameters.AccessModifier.GetDescription()} ");
        }

        if (string.IsNullOrEmpty(parameters.GenericTypeName))
        {
            sb.Append($"{parameters.TypeName} ");
        }
        else
        {
            sb.Append($"{parameters.GenericTypeName}<{parameters.TypeName}> ");
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
                sb.Append($" => {parameters.Content};");
            }
        }
        else if (!string.IsNullOrEmpty(parameters.Content))
        {
            sb.AppendLine();
            if (parameters.UseExpressionBody)
            {
                var lines = parameters.Content
                    .EnsureEnvironmentNewLines()
                    .Split(Environment.NewLine);

                if (lines.Length == 1)
                {
                    sb.Append(8, $"=> {lines[0]};");
                }
                else
                {
                    for (var i = 0; i < lines.Length; i++)
                    {
                        var line = lines[i];
                        if (i == 0)
                        {
                            sb.AppendLine(8, $"=> {line}");
                        }
                        else if (i == lines.Length - 1)
                        {
                            sb.Append(8, $"{line};");
                        }
                        else
                        {
                            sb.AppendLine(8, line);
                        }
                    }
                }
            }
            else
            {
                AppendContent(sb, 8, parameters.Content);
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
                    sb.AppendLine(4, $"[{item.Name}]");
                }
                else
                {
                    sb.AppendLine(4, $"[{item.Name}({item.Content})]");
                }
            }
        }

        sb.Append("    ");
        if (parameters.AccessModifier != AccessModifiers.None)
        {
            sb.Append($"{parameters.AccessModifier.GetDescription()} ");
        }

        if (!string.IsNullOrEmpty(parameters.ReturnTypeName))
        {
            if (string.IsNullOrEmpty(parameters.ReturnGenericTypeName))
            {
                sb.Append($"{parameters.ReturnTypeName} ");
            }
            else
            {
                sb.Append($"{parameters.ReturnGenericTypeName}<{parameters.ReturnTypeName}> ");
            }
        }

        sb.Append(parameters.Name);
        if (parameters.Parameters is not null &&
            parameters.Parameters.Any())
        {
            var indentSpaces = 0;
            if (parameters.Parameters.Count == 1)
            {
                sb.Append('(');
            }
            else
            {
                sb.AppendLine("(");
                indentSpaces = 8;
            }

            for (var i = 0; i < parameters.Parameters.Count; i++)
            {
                var item = parameters.Parameters[i];
                var useCommaForEndChar = i != parameters.Parameters.Count - 1;
                AppendInputParameter(
                    sb,
                    indentSpaces,
                    item.Attributes,
                    item.GenericTypeName,
                    item.TypeName,
                    item.Name,
                    item.DefaultValue,
                    useCommaForEndChar);
            }

            if (parameters.AccessModifier == AccessModifiers.None)
            {
                sb.Append(';');
            }
            else
            {
                if (string.IsNullOrEmpty(parameters.Content))
                {
                    sb.AppendLine();
                    sb.AppendLine(4, "{");
                    sb.Append(4, "}");
                }
                else
                {
                    if (parameters.UseExpressionBody)
                    {
                        sb.AppendLine();
                        sb.Append(8, "=> ");
                        AppendContent(sb, 0, parameters.Content);
                        sb.Append(';');
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.AppendLine(4, "{");
                        AppendContent(sb, 8, parameters.Content);
                        sb.AppendLine();
                        sb.Append(4, "}");
                    }
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

    public string GenerateMethodeToString(
        IList<PropertyParameters> parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        sb.AppendLine(4, "/// <inheritdoc />");
        sb.AppendLine(4, "public override string ToString()");
        sb.Append(8, "=> $\"");
        for (var i = 0; i < parameters.Count; i++)
        {
            var parametersProperty = parameters[i];
            sb.Append($"{{nameof({parametersProperty.Name})}}: {{{parametersProperty.Name}}}");
            if (i != parameters.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append("\";");

        return sb.ToString();
    }

    private static void AppendContent(
        StringBuilder sb,
        int indentSpaces,
        string content)
    {
        var lines = content
            .EnsureEnvironmentNewLines()
            .Split(Environment.NewLine);

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrEmpty(line))
            {
                sb.AppendLine();
            }
            else
            {
                if (i == lines.Length - 1)
                {
                    sb.Append(indentSpaces, line);
                }
                else
                {
                    sb.AppendLine(indentSpaces, line);
                }
            }
        }
    }

    private static void AppendInputParameter(
        StringBuilder sb,
        int indentSpaces,
        IList<AttributeParameters>? attributes,
        string? genericTypeName,
        string typeName,
        string name,
        string? defaultValue,
        bool useCommaForEndChar)
    {
        if (attributes is not null && attributes.Count == 1)
        {
            sb.Append(indentSpaces, $"[{attributes[0].Name}]");
            indentSpaces = 1;
        }

        if (string.IsNullOrEmpty(genericTypeName))
        {
            sb.Append(indentSpaces, $"{typeName} {name}");
        }
        else
        {
            sb.Append(indentSpaces, $"{genericTypeName}<{typeName}> {name}");
        }

        if (!string.IsNullOrEmpty(defaultValue))
        {
            sb.Append($" = {defaultValue}");
        }

        if (useCommaForEndChar)
        {
            sb.AppendLine(",");
        }
        else
        {
            sb.Append(')');
        }
    }
}
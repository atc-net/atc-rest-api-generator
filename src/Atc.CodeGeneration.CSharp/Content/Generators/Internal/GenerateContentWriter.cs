// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable InvertIf
// ReSharper disable StringLiteralTypo
namespace Atc.CodeGeneration.CSharp.Content.Generators.Internal;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OK.")]
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
            foreach (var attribute in attributes)
            {
                sb.AppendAttribute(usePropertyPrefix: false, attribute);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }

    public string GenerateConstructor(
        ConstructorParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        var sb = new StringBuilder();

        sb.AppendAccessModifier(4, parameters.AccessModifier);

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
                var firstParameterParameters = parameters.Parameters[0];
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
                    sb.AppendInputParameter(
                        8,
                        usePropertyPrefix: false,
                        attributes: null,
                        item.GenericTypeName,
                        item.TypeName,
                        item.IsNullableType,
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
                var firstParameterParameters = parameters.Parameters[0];
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

    public string GeneratePrivateReadonlyMembersToConstructor(
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
            sb.AppendAttributesAsLines(4, usePropertyPrefix: false, parameters.Attributes);
        }

        sb.Append("    ");
        if (parameters.AccessModifier != AccessModifiers.None)
        {
            sb.AppendAccessModifier(parameters.AccessModifier);
        }

        sb.AppendTypeAndName(parameters.GenericTypeName, parameters.TypeName, parameters.IsNullableType, parameters.Name);

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
                sb.AppendContentAsExpressionBody(1, parameters.Content);
            }
            else if (!string.IsNullOrEmpty(parameters.DefaultValue))
            {
                sb.Append($" = {parameters.DefaultValue};");
            }
        }
        else if (!string.IsNullOrEmpty(parameters.Content))
        {
            sb.AppendLine();
            if (parameters.UseExpressionBody)
            {
                sb.AppendContentAsExpressionBody(8, parameters.Content);
            }
            else
            {
                sb.AppendContent(8, parameters.Content);
            }
        }
        else
        {
            throw new ParametersException("Content is missing or use UseAutoProperty");
        }

        return sb.ToString();
    }

    public string GenerateMethod(
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
            sb.AppendAttributesAsLines(4, usePropertyPrefix: false, parameters.Attributes);
        }

        sb.Append("    ");
        if (parameters.AccessModifier != AccessModifiers.None)
        {
            sb.AppendAccessModifier(parameters.AccessModifier);
        }

        if (string.IsNullOrEmpty(parameters.ReturnTypeName))
        {
            sb.Append(parameters.Name);
        }
        else
        {
            sb.AppendTypeAndName(parameters.ReturnGenericTypeName, parameters.ReturnTypeName, isNullableType: false, parameters.Name);
        }

        if (parameters.Parameters is not null &&
            parameters.Parameters.Any())
        {
            var indentSpaces = 0;
            if (!parameters.AlwaysBreakDownParameters &&
                parameters.Parameters.Count == 1)
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
                sb.AppendInputParameter(
                    indentSpaces,
                    usePropertyPrefix: false,
                    item.Attributes,
                    item.GenericTypeName,
                    item.TypeName,
                    item.IsNullableType,
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
                        sb.AppendContentAsExpressionBody(8, parameters.Content);
                    }
                    else
                    {
                        sb.AppendLine();
                        sb.AppendLine(4, "{");
                        sb.AppendContent(8, parameters.Content);
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
                sb.AppendContent(parameters.Content);
            }
        }

        return sb.ToString();
    }

    public string GenerateMethodToString(
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
            if (parametersProperty.IsGenericListType)
            {
                sb.Append($"{{nameof({parametersProperty.Name})}}.Count: ");
                sb.Append($"{{{parametersProperty.Name}?.Count ?? 0}}");
            }
            else
            {
                sb.Append($"{{nameof({parametersProperty.Name})}}: ");
                if (parametersProperty.IsReferenceType)
                {
                    sb.Append($"({{{parametersProperty.Name}}})");
                }
                else
                {
                    sb.Append($"{{{parametersProperty.Name}}}");
                }
            }

            if (i != parameters.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append("\";");

        return sb.ToString();
    }
}
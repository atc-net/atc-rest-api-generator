namespace Atc.CodeGeneration.CSharp.Extensions;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class StringBuilderExtensions
{
    public static void AppendAccessModifier(
        this StringBuilder sb,
        AccessModifiers accessModifier)
        => sb.AppendAccessModifier(0, accessModifier);

    public static void AppendAccessModifier(
        this StringBuilder sb,
        int indentSpaces,
        AccessModifiers accessModifier)
    {
        if (accessModifier != AccessModifiers.None)
        {
            sb.Append(indentSpaces, $"{accessModifier.GetDescription()} ");
        }
    }

    public static void AppendTypeAndName(
        this StringBuilder sb,
        string? genericTypeName,
        string typeName,
        string name)
        => sb.AppendTypeAndName(
            0,
            genericTypeName,
            typeName,
            name,
            defaultValue: null);

    public static void AppendTypeAndName(
        this StringBuilder sb,
        int indentSpaces,
        string? genericTypeName,
        string typeName,
        string name)
        => sb.AppendTypeAndName(
            indentSpaces,
            genericTypeName,
            typeName,
            name,
            defaultValue: null);

    public static void AppendTypeAndName(
        this StringBuilder sb,
        int indentSpaces,
        string? genericTypeName,
        string typeName,
        string name,
        string? defaultValue)
    {
        sb.Append(
            indentSpaces,
            string.IsNullOrEmpty(genericTypeName)
                ? $"{typeName} {name}"
                : $"{genericTypeName}<{typeName}> {name}");

        if (!string.IsNullOrEmpty(defaultValue))
        {
            sb.Append($" = {defaultValue}");
        }
    }

    public static void AppendAttributesAsLines(
        this StringBuilder sb,
        int indentSpaces,
        bool usePropertyPrefix,
        IList<AttributeParameters> attributes)
    {
        foreach (var item in attributes)
        {
            sb.AppendAttribute(indentSpaces, usePropertyPrefix, item);
            sb.AppendLine();
        }
    }

    public static void AppendAttribute(
        this StringBuilder sb,
        bool usePropertyPrefix,
        AttributeParameters attribute)
        => sb.AppendAttribute(
            indentSpaces: 0,
            usePropertyPrefix,
            attribute.Name,
            attribute.Content);

    public static void AppendAttribute(
        this StringBuilder sb,
        int indentSpaces,
        bool usePropertyPrefix,
        AttributeParameters attribute)
        => sb.AppendAttribute(
            indentSpaces,
            usePropertyPrefix,
            attribute.Name,
            attribute.Content);

    public static void AppendAttribute(
        this StringBuilder sb,
        int indentSpaces,
        bool usePropertyPrefix,
        string name,
        string? content)
    {
        if (usePropertyPrefix)
        {
            sb.Append(
                indentSpaces,
                string.IsNullOrEmpty(content)
                    ? $"[property: {name}]"
                    : $"[property: {name}({content})]");
        }
        else
        {
            sb.Append(
                indentSpaces,
                string.IsNullOrEmpty(content)
                    ? $"[{name}]"
                    : $"[{name}({content})]");
        }
    }

    public static void AppendAttributes(
        this StringBuilder sb,
        int indentSpaces,
        bool usePropertyPrefix,
        bool mergeIntoOne,
        IList<AttributeParameters> attributes)
    {
        if (mergeIntoOne)
        {
            for (var i = 0; i < attributes.Count; i++)
            {
                if (i == 0)
                {
                    sb.Append(
                        indentSpaces,
                        usePropertyPrefix
                            ? "[property: "
                            : "[");
                }

                var attribute = attributes[i];

                if (string.IsNullOrEmpty(attribute.Content))
                {
                    sb.Append(attribute.Name);
                }
                else
                {
                    sb.Append($"{attribute.Name}({attribute.Content})");
                }

                if (i == attributes.Count - 1)
                {
                    sb.Append(']');
                }
                else
                {
                    sb.Append(", ");
                }
            }
        }
        else
        {
            foreach (var attribute in attributes)
            {
                AppendAttribute(sb, indentSpaces, usePropertyPrefix, attribute);
            }
        }
    }

    public static void AppendInputParameter(
        this StringBuilder sb,
        int indentSpaces,
        bool usePropertyPrefix,
        IList<AttributeParameters>? attributes,
        string? genericTypeName,
        string typeName,
        string name,
        string? defaultValue,
        bool useCommaForEndChar)
    {
        if (attributes is not null &&
            attributes.Count > 0)
        {
            switch (attributes.Count)
            {
                case 1:
                    sb.AppendAttribute(indentSpaces, usePropertyPrefix, attributes[0]);
                    indentSpaces = 1;
                    break;
                case > 1:
                    sb.AppendAttributes(indentSpaces, usePropertyPrefix, mergeIntoOne: true, attributes);
                    indentSpaces = 1;
                    break;
            }
        }

        sb.AppendTypeAndName(indentSpaces, genericTypeName, typeName, name, defaultValue);

        if (useCommaForEndChar)
        {
            sb.AppendLine(",");
        }
        else
        {
            sb.Append(')');
        }
    }

    public static void AppendContent(
        this StringBuilder sb,
        string content)
        => sb.AppendContent(0, content);

    public static void AppendContent(
        this StringBuilder sb,
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

    public static void AppendContentAsExpressionBody(
        this StringBuilder sb,
        int indentSpaces,
        string content)
    {
        var lines = content
            .EnsureEnvironmentNewLines()
            .Split(Environment.NewLine);

        if (lines.Length == 1)
        {
            sb.Append(indentSpaces, $"=> {lines[0]};");
        }
        else
        {
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (i == 0)
                {
                    sb.AppendLine(indentSpaces, $"=> {line}");
                }
                else if (i == lines.Length - 1)
                {
                    sb.Append(indentSpaces, $"{line};");
                }
                else
                {
                    sb.AppendLine(indentSpaces, line);
                }
            }
        }
    }
}
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
        IList<AttributeParameters> attributes)
    {
        foreach (var item in attributes)
        {
            sb.AppendAttribute(indentSpaces, item);
            sb.AppendLine();
        }
    }

    public static void AppendAttribute(
        this StringBuilder sb,
        AttributeParameters attribute)
        => sb.AppendAttribute(
            indentSpaces: 0,
            attribute.Name,
            attribute.Content);

    public static void AppendAttribute(
        this StringBuilder sb,
        int indentSpaces,
        AttributeParameters attribute)
        => sb.AppendAttribute(
            indentSpaces,
            attribute.Name,
            attribute.Content);

    public static void AppendAttribute(
        this StringBuilder sb,
        int indentSpaces,
        string name,
        string? content)
    {
        sb.Append(
            indentSpaces,
            string.IsNullOrEmpty(content)
                ? $"[{name}]"
                : $"[{name}({content})]");
    }

    public static void AppendInputParameter(
        this StringBuilder sb,
        int indentSpaces,
        IList<AttributeParameters>? attributes,
        string? genericTypeName,
        string typeName,
        string name,
        string? defaultValue,
        bool useCommaForEndChar)
    {
        if (attributes is not null &&
            attributes.Count == 1)
        {
            sb.AppendAttribute(indentSpaces, attributes[0]);
            indentSpaces = 1;
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
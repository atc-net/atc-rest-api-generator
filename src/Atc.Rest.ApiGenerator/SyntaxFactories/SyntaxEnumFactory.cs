using Atc.CodeAnalysis.CSharp.Factories;

// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.SyntaxFactories;

internal static class SyntaxEnumFactory
{
    public static EnumDeclarationSyntax Create(
        string title,
        OpenApiSchema apiSchema)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(apiSchema);

        return Create(title, apiSchema.Enum);
    }

    public static EnumDeclarationSyntax Create(
        string title,
        IList<IOpenApiAny> apiSchemaEnums)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(apiSchemaEnums);

        // Create an enum
        var enumDeclaration = SyntaxFactory.EnumDeclaration(title)
            .AddModifiers(SyntaxTokenFactory.PublicKeyword());

        // Find values to the enum
        var containTypeName = false;
        var lines = new List<string>();
        var intValues = new List<int>();
        foreach (var item in apiSchemaEnums)
        {
            if (item is not OpenApiString openApiString)
            {
                continue;
            }

            lines.Add(openApiString.Value.Trim());

            if (!openApiString.Value.Contains('=', StringComparison.Ordinal))
            {
                continue;
            }

            var sa = openApiString.Value.Split('=', StringSplitOptions.RemoveEmptyEntries);
            var s = sa.Last().Trim();
            if (int.TryParse(s, out var val))
            {
                intValues.Add(val);
            }
        }

        // Add values to the enum
        foreach (var line in lines)
        {
            enumDeclaration = enumDeclaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(line));

            var sa = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (sa.Length > 0 && sa.First().Equals("Object", StringComparison.Ordinal))
            {
                containTypeName = true;
            }
        }

        enumDeclaration = enumDeclaration
            .AddSuppressMessageAttribute(SuppressMessageAttributeFactory.CreateStyleCopSuppression(1413, justification: null));

        // Add SuppressMessageAttribute
        if (containTypeName)
        {
            enumDeclaration = enumDeclaration
                .AddSuppressMessageAttribute(SuppressMessageAttributeFactory.CreateCodeAnalysisSuppression(1720, justification: null));
        }

        // Add FlagAttribute
        if (intValues.Count > 0)
        {
            var isAllValidBinarySequence = intValues
                .Where(x => x != 0)
                .All(x => x.IsBinarySequence());
            if (isAllValidBinarySequence)
            {
                enumDeclaration = enumDeclaration.AddFlagAttribute();
            }
        }

        return enumDeclaration;
    }
}
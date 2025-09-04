namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;

public static class ContentGeneratorServerClientEnumParametersFactory
{
    public static EnumParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters codeGeneratorAttribute,
        string enumerationName,
        IList<IOpenApiAny> apiSchemaEnums)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaEnums);

        var documentationTags = new CodeDocumentationTags($"Enumeration: {enumerationName}.");

        var nameAndValues = ExtractNameAndValues(apiSchemaEnums);

        return EnumParametersFactory.Create(
            headerContent,
            @namespace,
            documentationTags,
            new List<AttributeParameters> { codeGeneratorAttribute },
            enumerationName,
            nameAndValues);
    }

    private static IDictionary<string, int?> ExtractNameAndValues(
        IEnumerable<IOpenApiAny> apiSchemaEnums)
    {
        var result = new Dictionary<string, int?>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in apiSchemaEnums)
        {
            if (item is not OpenApiString openApiString)
            {
                continue;
            }

            var line = openApiString.Value.Trim();

            var sa = line.Split('=', StringSplitOptions.RemoveEmptyEntries);
            switch (sa.Length)
            {
                case 1:
                    result.Add(
                        key: sa[0].Trim().Replace(":", "-", StringComparison.CurrentCulture),
                        value: null);
                    break;
                case 2:
                    result.Add(
                        key: sa[0].Trim(),
                        value: int.Parse(sa[1].Trim(), NumberStyles.Integer, GlobalizationConstants.EnglishCultureInfo));
                    break;
            }
        }

        return result;
    }
}
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerEnumerationParametersFactory
{
    public static ContentGeneratorServerEnumerationParameters Create(
        string @namespace,
        string enumerationName,
        IList<IOpenApiAny> apiSchemaEnums)
    {
        ArgumentNullException.ThrowIfNull(apiSchemaEnums);

        var useFlag = AnalyzeForUseFlagAttribute(apiSchemaEnums);
        var enumerationValues = GetContentGeneratorServerEnumerationParametersValues(apiSchemaEnums);

        var documentationTags = new CodeDocumentationTags($"Enumeration: {enumerationName}.");

        return new ContentGeneratorServerEnumerationParameters(
            @namespace,
            enumerationName,
            documentationTags,
            useFlag,
            enumerationValues);
    }

    private static List<ContentGeneratorServerEnumerationParametersValue> GetContentGeneratorServerEnumerationParametersValues(
        IEnumerable<IOpenApiAny> apiSchemaEnums)
    {
        var result = new List<ContentGeneratorServerEnumerationParametersValue>();

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
                    result.Add(new ContentGeneratorServerEnumerationParametersValue(
                        Name: sa[0].Trim(),
                        Value: null));
                    break;
                case 2:
                    result.Add(new ContentGeneratorServerEnumerationParametersValue(
                        Name: sa[0].Trim(),
                        Value: int.Parse(sa[1].Trim(), NumberStyles.Integer, GlobalizationConstants.EnglishCultureInfo)));
                    break;
            }
        }

        return result;
    }

    private static bool AnalyzeForUseFlagAttribute(
        IEnumerable<IOpenApiAny> apiSchemaEnums)
    {
        var intValues = new List<int>();
        foreach (var item in apiSchemaEnums)
        {
            if (item is not OpenApiString openApiString)
            {
                continue;
            }

            if (!openApiString.Value.Contains('=', StringComparison.Ordinal))
            {
                continue;
            }

            var sa = openApiString.Value.Split('=', StringSplitOptions.RemoveEmptyEntries);
            var s = sa.Last().Trim();
            if (int.TryParse(s, NumberStyles.Integer, GlobalizationConstants.EnglishCultureInfo, out var val))
            {
                intValues.Add(val);
            }
        }

        if (intValues.Count <= 0)
        {
            return false;
        }

        return intValues
            .Where(x => x != 0)
            .All(x => x.IsBinarySequence());
    }
}
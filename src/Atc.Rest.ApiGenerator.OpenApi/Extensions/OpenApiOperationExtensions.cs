namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiOperationExtensions
{
    public static string GetOperationSummaryDescription(
        this OpenApiOperation apiOperation)
    {
        var result = apiOperation.Summary;

        if (string.IsNullOrEmpty(result))
        {
            result = apiOperation.Description;
        }

        if (string.IsNullOrEmpty(result))
        {
            return ContentGeneratorConstants.UndefinedDescription;
        }

        if (!result.EndsWith('.'))
        {
            result += ".";
        }

        return result.Replace("\n", "\n/// ", StringComparison.Ordinal);
    }
}
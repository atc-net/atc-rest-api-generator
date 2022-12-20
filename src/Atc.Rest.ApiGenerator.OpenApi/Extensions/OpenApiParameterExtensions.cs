namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiParameterExtensions
{
    public static string GetOperationSummaryDescription(
        this OpenApiParameter apiOperation)
    {
        var result = apiOperation.Description;

        if (string.IsNullOrEmpty(result))
        {
            return ContentGeneratorConstants.UndefinedDescription;
        }

        if (!result.EndsWith('.'))
        {
            result += ".";
        }

        return result;
    }
}
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Helpers;

public static class OpenApiDocumentHelper
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:Parameter should not span multiple lines", Justification = "OK.")]
    public static bool Validate(
        ILogger logger,
        OpenApiDocumentContainer apiDocumentContainer,
        ApiOptionsValidation validationOptions)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(apiDocumentContainer);
        ArgumentNullException.ThrowIfNull(validationOptions);

        logger.LogInformation($"{AppEmojisConstants.AreaValidation} Working on validation");

        if (apiDocumentContainer.Diagnostic!.Errors.Count > 0)
        {
            var validationErrors = apiDocumentContainer.Diagnostic.Errors
                .Where(e => !e.Message.EndsWith(
                    "#/components/schemas",
                    StringComparison.Ordinal))
                .Select(e => LogItemHelper.Create(
                    LogCategoryType.Error,
                    ValidationRuleNameConstants.OpenApiCore,
                    string.IsNullOrEmpty(e.Pointer)
                        ? $"{e.Message}"
                        : $"{e.Message} <#> {e.Pointer}"))
                .ToList();

            logger.LogKeyValueItems(validationErrors);
            return false;
        }

        if (apiDocumentContainer.Diagnostic.SpecificationVersion == OpenApiSpecVersion.OpenApi2_0)
        {
            logger.LogError("OpenApi 2.x is not supported.");
            return false;
        }

        return OpenApiDocumentValidationHelper.ValidateDocument(
            logger,
            apiDocumentContainer.Document!,
            validationOptions);
    }
}
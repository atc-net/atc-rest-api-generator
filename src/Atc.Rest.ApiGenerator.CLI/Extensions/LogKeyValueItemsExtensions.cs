namespace Atc.Rest.ApiGenerator.CLI.Extensions;

public static class LogKeyValueItemsExtensions
{
    public static bool HasAnyErrorsLogIfNeeded(
        this List<LogKeyValueItem> logItems,
        ILogger logger)
    {
        if (logItems.Any(x => x.LogCategory == LogCategoryType.Error))
        {
            logger.LogKeyValueItems(logItems);
            return true;
        }

        return false;
    }
}
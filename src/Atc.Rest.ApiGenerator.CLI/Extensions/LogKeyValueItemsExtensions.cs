// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.CLI.Extensions;

public static class LogKeyValueItemsExtensions
{
    public static bool HasAnyErrors(
        this IEnumerable<LogKeyValueItem> logItems)
        => logItems.Any(x => x.LogCategory == LogCategoryType.Error);

    public static void Log(
        this List<LogKeyValueItem> logItems,
        ILogger logger)
    {
        if (!logItems.Any())
        {
            return;
        }

        logger.LogKeyValueItems(logItems);
    }

    public static void LogAndClear(
        this List<LogKeyValueItem> logItems,
        ILogger logger)
    {
        if (!logItems.Any())
        {
            return;
        }

        logger.LogKeyValueItems(logItems);
        logItems.Clear();
    }
}
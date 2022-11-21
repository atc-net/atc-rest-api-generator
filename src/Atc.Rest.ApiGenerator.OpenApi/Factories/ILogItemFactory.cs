namespace Atc.Rest.ApiGenerator.OpenApi.Factories;

public interface ILogItemFactory
{
    LogKeyValueItem Create(
        LogCategoryType logCategoryType,
        string ruleName,
        string description);
}
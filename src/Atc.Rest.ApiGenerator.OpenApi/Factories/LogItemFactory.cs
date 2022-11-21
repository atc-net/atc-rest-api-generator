namespace Atc.Rest.ApiGenerator.OpenApi.Factories;

public class LogItemFactory : ILogItemFactory
{
    public LogKeyValueItem Create(
        LogCategoryType logCategoryType,
        string ruleName,
        string description)
        => Data.LogItemFactory.Create(logCategoryType, ruleName, ExtractAreaFromRuleName(ruleName), description);

    private static string ExtractAreaFromRuleName(
        string ruleName)
    {
        var result = "#";
        var fieldInfos = typeof(ValidationRuleNameConstants).GetFields();
        var list = new List<Tuple<string, string>>();
        foreach (var fieldInfo in fieldInfos)
        {
            var o = fieldInfo.GetValue(null);
            var fieldValue = string.Empty;
            if (o is not null)
            {
                fieldValue = o.ToString();
            }

            list.Add(new Tuple<string, string>(fieldInfo.Name, fieldValue!));
        }

        var tuple = list.FirstOrDefault(x => x.Item2 == ruleName);
        if (tuple is null ||
            string.IsNullOrEmpty(tuple.Item2))
        {
            return result;
        }

        result = tuple.Item1;
        if (char.IsDigit(result[^2]))
        {
            result = result.Substring(0, result.Length - 2);
        }

        return result;
    }
}
namespace Atc.Rest.ApiGenerator.CodingRules.Extensions;

public static class StringExtensions
{
    private static readonly string[] LineBreaks = ["\r\n", "\r", "\n"];

    public static string TrimEndForEmptyLines(
        this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }

        var values = value
            .Split(LineBreaks, StringSplitOptions.None)
            .ToList();

        values.TrimEndForEmptyValues();
        return string.Join(Environment.NewLine, values);
    }
}
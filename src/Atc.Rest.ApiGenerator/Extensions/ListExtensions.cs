namespace Atc.Rest.ApiGenerator.Extensions;

public static class ListExtensions
{
    public static void TrimEndForEmptyValues(this IList<string> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var tryAgain = true;
        while (tryAgain)
        {
            if (values.Count == 0)
            {
                tryAgain = false;
            }
            else
            {
                var lastLine = values.Last().Trim();
                if (lastLine.Length == 0)
                {
                    values.RemoveAt(values.Count - 1);
                }
                else
                {
                    tryAgain = false;
                }
            }
        }
    }
}
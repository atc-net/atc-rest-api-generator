namespace Atc.CodeGeneration.CSharp.Extensions;

public static class RegularExpressionAttributeExtensions
{
    public static string GetEscapedPattern(
        this RegularExpressionAttribute regularExpressionAttribute,
        bool ensureQuotes = true)
    {
        ArgumentNullException.ThrowIfNull(regularExpressionAttribute);

        if (!regularExpressionAttribute.Pattern.Contains(@"\\", StringComparison.Ordinal))
        {
            return SymbolDisplay.FormatLiteral(regularExpressionAttribute.Pattern, quote: ensureQuotes);
        }

        if (ensureQuotes &&
            !regularExpressionAttribute.Pattern.StartsWith('"') &&
            !regularExpressionAttribute.Pattern.EndsWith('"'))
        {
            return $"\"{regularExpressionAttribute.Pattern}\"";
        }

        return regularExpressionAttribute.Pattern;
    }
}
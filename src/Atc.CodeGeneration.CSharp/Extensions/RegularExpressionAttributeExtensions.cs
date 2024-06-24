namespace Atc.CodeGeneration.CSharp.Extensions;

public static class RegularExpressionAttributeExtensions
{
    public static string GetEscapedPattern(
        this RegularExpressionAttribute regularExpressionAttribute,
        bool ensureQuote = true)
    {
        ArgumentNullException.ThrowIfNull(regularExpressionAttribute);

        if (!regularExpressionAttribute.Pattern.Contains(@"\\", StringComparison.Ordinal))
        {
            return SymbolDisplay.FormatLiteral(regularExpressionAttribute.Pattern, quote: ensureQuote);
        }

        if (ensureQuote &&
            !regularExpressionAttribute.Pattern.StartsWith('"') &&
            !regularExpressionAttribute.Pattern.EndsWith('"'))
        {
            return $"\"{regularExpressionAttribute.Pattern}\"";
        }

        return regularExpressionAttribute.Pattern;
    }
}
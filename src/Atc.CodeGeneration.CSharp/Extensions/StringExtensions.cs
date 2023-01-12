namespace Atc.CodeGeneration.CSharp.Extensions;

public static class StringExtensions
{
    public static string EnsureValidFormattedPropertyName(
        this string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (name.StartsWith("x-", StringComparison.OrdinalIgnoreCase))
        {
            name = name[2..];
        }

        if (name.Contains('-', StringComparison.Ordinal))
        {
            return name
                .PascalCase(
                    separators: new[] { '-' },
                    removeSeparators: true);
        }

        return name.EnsureFirstCharacterToUpper();
    }
}
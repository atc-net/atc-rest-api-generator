// ReSharper disable once CheckNamespace
namespace System;

internal static class StringExtensions
{
    private static readonly string[] LineBreaks = { "\r\n", "\r", "\n" };

    public static string EnsureNewlineAfterMethod(
        this string value,
        string methodName)
        => value.Replace(methodName, methodName + Environment.NewLine, StringComparison.Ordinal);

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
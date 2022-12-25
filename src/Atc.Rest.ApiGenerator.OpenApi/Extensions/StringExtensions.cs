// ReSharper disable CheckNamespace
namespace System;

public static class StringExtensions
{
    // TODO: Move to ATC.
    public static string EnsureEndsWithDot(
        this string value)
        => value.EndsWith('.')
            ? value
            : $"{value}.";
}
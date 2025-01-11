namespace Atc.Rest.ApiGenerator.Contracts.Extensions;

public static class StringExtensions
{
    public static bool IsWellKnownSystemTypeName(
        this string value)
        => value is not null &&
           (value.EndsWith("Task", StringComparison.Ordinal) ||
            value.EndsWith("Tasks", StringComparison.Ordinal) ||
            value.EndsWith("Endpoint", StringComparison.Ordinal) ||
            value.EndsWith("EventArgs", StringComparison.Ordinal));
}
namespace Atc.Rest.ApiGenerator.CLI.Commands.Attributes;

/// <summary>
/// SwaggerThemeMode Description Attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Delegate)]
public sealed class SwaggerThemeModeAttribute : DescriptionAttribute
{
    /// <summary>Gets or sets the default.</summary>
    /// <value>The default.</value>
    public new SwaggerThemeMode Default { get; set; } = SwaggerThemeMode.None;

    /// <summary>Gets or sets the prefix.</summary>
    /// <value>The prefix.</value>
    public string? Prefix { get; set; }

    /// <summary>
    /// Gets the description stored in this attribute.
    /// </summary>
    /// <returns>The description stored in this attribute.</returns>
    public override string Description
    {
        get
        {
            var values = Enum.GetNames<SwaggerThemeMode>()
                .Select(enumValue => enumValue.Equals(Default.ToString(), StringComparison.Ordinal)
                    ? $"{enumValue} (default)"
                    : enumValue)
                .ToList();

            return string.IsNullOrEmpty(Prefix)
                ? $"Valid values are: {string.Join(", ", values)}"
                : $"{Prefix}. Valid values are: {string.Join(", ", values)}".Replace("..", ".", StringComparison.Ordinal);
        }
    }
}
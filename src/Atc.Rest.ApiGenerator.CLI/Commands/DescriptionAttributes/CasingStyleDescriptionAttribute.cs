namespace Atc.Rest.ApiGenerator.CLI.Commands.DescriptionAttributes;

public sealed class CasingStyleDescriptionAttribute : DescriptionAttribute
{
    public new CasingStyle Default { get; set; }

    public string Prefix { get; set; }

    public override string Description
    {
        get
        {
            var values = Enum.GetNames(typeof(CasingStyle))
                .Where(enumValue => enumValue != CasingStyle.None.ToString())
                .Select(enumValue => enumValue.Equals(Default.ToString(), StringComparison.Ordinal)
                    ? $"{enumValue} (default)"
                    : enumValue)
                .ToList();

            return $"{Prefix}. Valid values are: " +
                   string.Join(", ", values);
        }
    }
}
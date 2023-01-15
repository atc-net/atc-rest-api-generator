// ReSharper disable UnusedVariable
namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class AttributesParametersFactory
{
    public static IList<AttributeParameters> Create(
        string name)
        => new List<AttributeParameters>
        {
            new(name, Content: null),
        };

    public static IList<AttributeParameters> Create(
        string name,
        string content)
        => new List<AttributeParameters>
        {
            new(name, content),
        };

    public static IList<AttributeParameters> Create(
        IList<ValidationAttribute> validationAttributes)
    {
        ArgumentNullException.ThrowIfNull(validationAttributes);

        var result = new List<AttributeParameters>();
        foreach (var validationAttribute in validationAttributes)
        {
            switch (validationAttribute)
            {
                case StringLengthAttribute stringLengthAttribute:
                    result.Add(new AttributeParameters("StringLength", stringLengthAttribute.MaximumLength.ToString()));
                    break;
                case MinLengthAttribute minLengthAttribute:
                    result.Add(new AttributeParameters("MinLength", minLengthAttribute.Length.ToString()));
                    break;
                case MaxLengthAttribute maxLengthAttribute:
                    result.Add(new AttributeParameters("MaxLength", maxLengthAttribute.Length.ToString()));
                    break;
                case RangeAttribute rangeAttribute:
                    result.Add(new AttributeParameters("Range", $"{rangeAttribute.Minimum}, {rangeAttribute.Maximum}"));
                    break;
                case RegularExpressionAttribute regularExpressionAttribute:
                    result.Add(new AttributeParameters("RegularExpression", $"\"{regularExpressionAttribute.Pattern}\""));
                    break;
                case UriAttribute:
                    result.Add(new AttributeParameters("Uri", Content: null));
                    break;
                case EmailAddressAttribute:
                    result.Add(new AttributeParameters("EmailAddress", Content: null));
                    break;
            }
        }

        return result;
    }
}
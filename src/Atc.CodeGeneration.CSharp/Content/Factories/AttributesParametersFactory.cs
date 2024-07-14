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
                    var minimum = rangeAttribute.Minimum;
                    var maximum = rangeAttribute.Maximum;

                    if (rangeAttribute.OperandType == typeof(double))
                    {
                        if (double.MinValue.Equals((double)rangeAttribute.Minimum) ||
                            rangeAttribute.Minimum is double.NegativeInfinity)
                        {
                            minimum = "double.MinValue";
                        }

                        if (double.MaxValue.Equals((double)rangeAttribute.Maximum) ||
                            rangeAttribute.Maximum is double.PositiveInfinity)
                        {
                            maximum = "double.MaxValue";
                        }
                    }
                    else if (rangeAttribute.OperandType == typeof(int))
                    {
                        if (int.MinValue.Equals((int)rangeAttribute.Minimum))
                        {
                            minimum = "int.MinValue";
                        }

                        if (int.MaxValue.Equals((int)rangeAttribute.Maximum))
                        {
                            maximum = "int.MaxValue";
                        }
                    }
                    else if (rangeAttribute.OperandType == typeof(long))
                    {
                        if (long.MinValue.Equals((long)rangeAttribute.Minimum))
                        {
                            minimum = "long.MinValue";
                        }

                        if (long.MaxValue.Equals((long)rangeAttribute.Maximum))
                        {
                            maximum = "long.MaxValue";
                        }
                    }

                    result.Add(new AttributeParameters("Range", $"{minimum}, {maximum}"));
                    break;
                case RegularExpressionAttribute regularExpressionAttribute:
                    result.Add(new AttributeParameters("RegularExpression", regularExpressionAttribute.GetEscapedPattern()));
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
namespace Atc.Rest.ApiGenerator.OpenApi.Extractors;

public sealed class ValidationAttributesExtractor : IValidationAttributesExtractor
{
    public IList<ValidationAttribute> Extract(
        OpenApiSchema openApiSchema)
    {
        ArgumentNullException.ThrowIfNull(openApiSchema);

        var validationAttributes = new List<ValidationAttribute>();

        AppendAttributeFromSchemaFormatIfRequired(validationAttributes, openApiSchema);
        AppendAttributeForMinMaxIfRequired(validationAttributes, openApiSchema);
        AppendAttributeForPatternIfRequired(validationAttributes, openApiSchema);

        return validationAttributes;
    }

    private static void AppendAttributeFromSchemaFormatIfRequired(
        ICollection<ValidationAttribute> validationAttributes,
        OpenApiSchema schema)
    {
        if (string.IsNullOrEmpty(schema.Format))
        {
            return;
        }

        var format = schema.Format.ToLower(CultureInfo.CurrentCulture);
        switch (format)
        {
            case OpenApiFormatTypeConstants.Uuid:
            case OpenApiFormatTypeConstants.Date:
            case OpenApiFormatTypeConstants.Time:
            case OpenApiFormatTypeConstants.Timestamp:
            case OpenApiFormatTypeConstants.DateTime:
            case OpenApiFormatTypeConstants.Byte:
            case OpenApiFormatTypeConstants.Binary:
            case OpenApiFormatTypeConstants.Int32:
            case OpenApiFormatTypeConstants.Int64:
            case OpenApiFormatTypeConstants.Float:
            case OpenApiFormatTypeConstants.Double:
                break;
            case OpenApiFormatTypeConstants.Email:
                validationAttributes.Add(new EmailAddressAttribute());
                if (!string.IsNullOrEmpty(schema.Pattern))
                {
                    validationAttributes.Add(new RegularExpressionAttribute(schema.Pattern));
                }

                break;
            case OpenApiFormatTypeConstants.Uri:
                validationAttributes.Add(new UriAttribute());
                break;
            default:
                throw new NotImplementedException($"Schema Format '{format}' must be implemented.");
        }
    }

    private static void AppendAttributeForMinMaxIfRequired(
        ICollection<ValidationAttribute> validationAttributes,
        OpenApiSchema schema)
    {
        if (schema.Type == OpenApiDataTypeConstants.String &&
            schema.MinLength is null &&
            schema.MaxLength is not null)
        {
            validationAttributes.Add(new StringLengthAttribute(schema.MaxLength.Value));
        }
        else
        {
            if (schema.MinLength is > 0)
            {
                validationAttributes.Add(new MinLengthAttribute(schema.MinLength.Value));
            }

            if (schema.MaxLength is > 0)
            {
                validationAttributes.Add(new MaxLengthAttribute(schema.MaxLength.Value));
            }

            if (schema.Minimum == null &&
                schema.Maximum == null)
            {
                return;
            }

            switch (schema.Type)
            {
                case OpenApiDataTypeConstants.Number:
                    if (!schema.HasFormatType())
                    {
                        validationAttributes.Add(CreateRangeAttributeDouble(schema));
                    }

                    break;
                case OpenApiDataTypeConstants.Integer:
                    if (schema.HasFormatType() && schema.IsFormatTypeInt64())
                    {
                        validationAttributes.Add(CreateRangeAttributeLong(schema));
                    }
                    else
                    {
                        validationAttributes.Add(CreateRangeAttributeInt(schema));
                    }

                    break;
            }
        }
    }

    private static void AppendAttributeForPatternIfRequired(
        ICollection<ValidationAttribute> validationAttributes,
        OpenApiSchema schema)
    {
        if (schema.Type == OpenApiDataTypeConstants.String &&
            schema.Pattern is not null)
        {
            validationAttributes.Add(new RegularExpressionAttribute(schema.Pattern));
        }
    }

    private static RangeAttribute CreateRangeAttributeDouble(
        OpenApiSchema schema)
    {
        double min;
        if (schema.Minimum.HasValue)
        {
            min = (double)schema.Minimum.Value;
        }
        else
        {
            min = double.NegativeInfinity;
        }

        double max;
        if (schema.Maximum.HasValue)
        {
            max = (double)schema.Maximum.Value;
        }
        else
        {
            max = double.PositiveInfinity;
        }

        if (max < min)
        {
            max = min;
        }

        return new RangeAttribute(min, max);
    }

    private static ValidationAttribute CreateRangeAttributeLong(
        OpenApiSchema schema)
    {
        long min;
        if (schema.Minimum.HasValue)
        {
            min = (long)schema.Minimum.Value;
        }
        else
        {
            min = int.MinValue;
        }

        long max;
        if (schema.Maximum.HasValue)
        {
            max = (long)schema.Maximum.Value;
        }
        else
        {
            max = long.MaxValue;
        }

        if (max < min)
        {
            max = min;
        }

        return new RangeAttribute(min, max);
    }

    private static ValidationAttribute CreateRangeAttributeInt(
        OpenApiSchema schema)
    {
        int min;
        if (schema.Minimum.HasValue)
        {
            min = (int)schema.Minimum.Value;
        }
        else
        {
            min = int.MinValue;
        }

        int max;
        if (schema.Maximum.HasValue)
        {
            max = (int)schema.Maximum.Value;
        }
        else
        {
            max = int.MaxValue;
        }

        if (max < min)
        {
            max = min;
        }

        return new RangeAttribute(min, max);
    }
}
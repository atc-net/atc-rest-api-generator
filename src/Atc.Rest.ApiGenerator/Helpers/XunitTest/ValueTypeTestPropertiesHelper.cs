// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest;

public static class ValueTypeTestPropertiesHelper
{
    public static string CreateValueEnum(
        string name,
        KeyValuePair<string, OpenApiSchema> schemaForEnum,
        bool useForBadRequest)
    {
        var enumSchema = schemaForEnum.Value.GetEnumSchema();
        var enumValues = enumSchema.Item2.Enum.ToArray();
        if (enumValues.Last() is not OpenApiString openApiString)
        {
            throw new NotSupportedException($"PropertyValueGeneratorEnum: {name}");
        }

        return useForBadRequest
            ? "@"
            : openApiString.Value;
    }

    public static string Number(
        string name,
        OpenApiSchema schema,
        bool useForBadRequest)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(schema);

        if (useForBadRequest)
        {
            return "@";
        }

        if (name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("Id", StringComparison.Ordinal))
        {
            return "27";
        }

        return schema.Type switch
        {
            OpenApiDataTypeConstants.Number when !schema.HasFormatType() => CreateNumberDouble(schema),
            OpenApiDataTypeConstants.Integer when schema.HasFormatType() && schema.IsFormatTypeInt64() => CreateNumberLong(schema),
            _ => CreateNumberInt(schema)
        };
    }

    public static string CreateValueBool(bool useForBadRequest)
        => useForBadRequest ? "@" : "true";

    public static string CreateValueString(
        string name,
        OpenApiSchema schema,
        ParameterLocation? parameterLocation,
        bool useForBadRequest,
        int itemNumber = 0,
        string? customValue = null)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(schema);

        if (!string.IsNullOrEmpty(schema.Format))
        {
            if (schema.Format.Equals(OpenApiFormatTypeConstants.Email, StringComparison.OrdinalIgnoreCase))
            {
                return CreateValueEmail(useForBadRequest, itemNumber);
            }

            if (schema.Format.Equals(OpenApiFormatTypeConstants.Uri, StringComparison.OrdinalIgnoreCase))
            {
                return CreateValueUri(useForBadRequest);
            }
        }

        if (useForBadRequest &&
            parameterLocation == ParameterLocation.Query)
        {
            return string.Empty;
        }

        if (name.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
            name.EndsWith("Id", StringComparison.Ordinal))
        {
            return CreateValueStringId(useForBadRequest, itemNumber, customValue);
        }

        var val = CreateValueStringDefault(useForBadRequest, itemNumber, customValue);
        if (!useForBadRequest &&
            schema.IsRuleValidationString())
        {
            var min = schema.MinLength ?? 0;
            if (val.Length < min)
            {
                val = val.PadRight(min, 'X');
            }
            else
            {
                var max = schema.MaxLength ?? 20;
                if (val.Length > max)
                {
                    val = val.Substring(0, max);
                }
            }
        }

        return val;
    }

    public static string CreateValueDateTimeOffset(
        bool useForBadRequest,
        int itemNumber = 0)
    {
        var sec = 23 + itemNumber;
        return useForBadRequest
            ? $"x2020-10-12T21:22:{sec:D2}"
            : $"2020-10-12T21:22:{sec:D2}";
    }

    public static string CreateValueGuid(
        bool useForBadRequest,
        int itemNumber = 0)
    {
        var numberPart = itemNumber.ToString(GlobalizationConstants.EnglishCultureInfo).PadLeft(4, '0');
        return useForBadRequest
            ? $"x77a33260-{numberPart}-441f-ba60-b0a833803fab"
            : $"77a33260-{numberPart}-441f-ba60-b0a833803fab";
    }

    [SuppressMessage("Design", "CA1055:URI-like return values should not be strings", Justification = "OK.")]
    public static string CreateValueUri(
        bool useForBadRequest)
        => useForBadRequest
            ? "http_www_dr_dk"
            : "http://www.dr.dk";

    public static string CreateValueEmail(
        bool useForBadRequest,
        int itemNumber = 0)
    {
        if (itemNumber > 0)
        {
            return useForBadRequest
                ? $"john{itemNumber}.doe_example.com"
                : $"john{itemNumber}.doe@example.com";
        }

        return useForBadRequest
            ? "john.doe_example.com"
            : "john.doe@example.com";
    }

    public static string CreateValueArray(
        string name,
        OpenApiSchema itemSchema,
        ParameterLocation? parameterLocation,
        bool useForBadRequest,
        int count)
        => Enumerable.Range(0, count)
            .Select(i => CreateValueArrayItem(name, itemSchema, parameterLocation, useForBadRequest, i))
            .Aggregate((item1, item2) => item1 + $"&{name}=" + item2);

    private static string CreateValueStringDefault(
        bool useForBadRequest,
        int itemNumber,
        string? customValue)
    {
        if (itemNumber > 0)
        {
            return useForBadRequest
                ? customValue ?? "null"
                : customValue ?? "Hallo" + itemNumber;
        }

        return useForBadRequest
            ? customValue ?? "null"
            : customValue ?? "Hallo";
    }

    private static string CreateValueStringId(
        bool useForBadRequest,
        int itemNumber,
        string? customValue)
    {
        if (itemNumber > 0)
        {
            return useForBadRequest
                ? customValue ?? "27@" + itemNumber
                : customValue ?? "27" + itemNumber;
        }

        return useForBadRequest
            ? customValue ?? "null"
            : customValue ?? "27";
    }

    private static string CreateNumberInt(
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

        if (min > 42)
        {
            return min.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        if (max < 42)
        {
            return max.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return "42";
    }

    private static string CreateNumberLong(
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

        if (min > 42)
        {
            return min.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        if (max < 42)
        {
            return max.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return "42";
    }

    private static string CreateNumberDouble(
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

        if (min > 42.2)
        {
            return min.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        if (max < 42.2)
        {
            return max.ToString(GlobalizationConstants.EnglishCultureInfo);
        }

        return "42.2";
    }

    private static string CreateValueArrayItem(
        string name,
        OpenApiSchema itemSchema,
        ParameterLocation? parameterLocation,
        bool useForBadRequest,
        int itemNumber = 0)
        => itemSchema.GetDataType() switch
        {
            "double" => Number(name, itemSchema, useForBadRequest),
            "long" => Number(name, itemSchema, useForBadRequest),
            "int" => Number(name, itemSchema, useForBadRequest),
            "bool" => CreateValueBool(useForBadRequest),
            "string" => CreateValueString(name, itemSchema, parameterLocation, useForBadRequest, itemNumber, customValue: null),
            "DateTimeOffset" => CreateValueDateTimeOffset(useForBadRequest, itemNumber),
            "Guid" => CreateValueGuid(useForBadRequest, itemNumber),
            "Uri" => CreateValueUri(useForBadRequest),
            "Email" => CreateValueEmail(useForBadRequest, itemNumber),
            _ => throw new NotSupportedException($"PropertyValueGenerator: {name} - array of ({itemSchema.GetDataType()})")
        };
}
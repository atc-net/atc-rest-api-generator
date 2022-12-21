// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable MergeIntoPattern
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters;

public static class ContentGeneratorServerHandlerParameterParametersFactory
{
    public static ContentGeneratorServerHandlerParameterParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation,
        IList<OpenApiParameter> globalPathParameters)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);
        ArgumentNullException.ThrowIfNull(globalPathParameters);

        var operationName = openApiOperation.GetOperationName();

        var parameters = new List<ContentGeneratorServerParameterParametersProperty>();

        AppendParameters(parameters, globalPathParameters);
        AppendParameters(parameters, openApiOperation.Parameters);
        AppendParametersFromBody(parameters, openApiOperation.RequestBody);

        return new ContentGeneratorServerHandlerParameterParameters(
            @namespace,
            operationName,
            openApiOperation.GetOperationSummaryDescription(),
            ParameterName: $"{operationName}{ContentGeneratorConstants.Parameters}",
            parameters);
    }

    private static void AppendParameters(
        ICollection<ContentGeneratorServerParameterParametersProperty> parameters,
        IEnumerable<OpenApiParameter> openApiParameters)
    {
        foreach (var openApiParameter in openApiParameters)
        {
            var useListForDataType = openApiParameter.Schema.IsTypeArray();

            var dataType = useListForDataType
                ? openApiParameter.Schema.Items.GetDataType()
                : openApiParameter.Schema.GetDataType();

            parameters.Add(new ContentGeneratorServerParameterParametersProperty(
                openApiParameter.Name,
                openApiParameter.Name.PascalCase(removeSeparators: true),
                openApiParameter.GetOperationSummaryDescription(),
                ConvertToParameterLocationType(openApiParameter.In),
                dataType,
                IsSimpleType: true,
                useListForDataType,
                GetIsNullable(openApiParameter, useListForDataType),
                openApiParameter.Required,
                GetAdditionalValidationAttributes(openApiParameter),
                GetDefaultValue(openApiParameter.Schema.Default)));
        }
    }

    private static bool GetIsNullable(
        OpenApiParameter openApiParameter,
        bool useListForDataType)
    {
        var isNullable = openApiParameter.Schema.Nullable;
        isNullable = isNullable switch
        {
            true when useListForDataType => false,
            false when openApiParameter.Schema.IsSchemaEnumOrPropertyEnum() => true,
            _ => isNullable,
        };

        return isNullable;
    }

    private static string? GetDefaultValue(
        IOpenApiAny? initializer)
    {
        if (initializer is null)
        {
            return null;
        }

        switch (initializer)
        {
            case OpenApiInteger apiInteger:
                return apiInteger.Value.ToString();
            case OpenApiString apiString:
                if (string.IsNullOrEmpty(apiString.Value))
                {
                    return "string.Empty";
                }

                return apiString.Value;
            case OpenApiBoolean apiBoolean when apiBoolean.Value:
                return "true";
            case OpenApiBoolean apiBoolean when !apiBoolean.Value:
                return "false";
            default:
                throw new NotImplementedException("Property initializer: " + initializer.GetType());
        }
    }

    private static void AppendParametersFromBody(
        ICollection<ContentGeneratorServerParameterParametersProperty> parameters,
        OpenApiRequestBody? requestBody)
    {
        var requestSchema = requestBody?.Content?.GetSchemaByFirstMediaType();

        if (requestSchema is null)
        {
            return;
        }

        var isFormatTypeOfBinary = requestSchema.IsFormatTypeBinary();
        var isItemsOfFormatTypeBinary = requestSchema.HasItemsWithFormatTypeBinary();

        var requestBodyType = "string?";
        if (requestSchema.Reference is not null)
        {
            requestBodyType = requestSchema.Reference.Id.EnsureFirstCharacterToUpper();
        }
        else if (isFormatTypeOfBinary)
        {
            requestBodyType = "IFormFile";
        }
        else if (isItemsOfFormatTypeBinary)
        {
            requestBodyType = "IFormFile";
        }
        else if (requestSchema.Items is not null)
        {
            requestBodyType = requestSchema.Items.Reference.Id.EnsureFirstCharacterToUpper();
        }

        parameters.Add(new ContentGeneratorServerParameterParametersProperty(
            string.Empty,
            ContentGeneratorConstants.Request,
            requestSchema.GetRequestBodySummaryDescription(),
            requestSchema.GetParameterLocationType(),
            requestBodyType,
            IsSimpleType: false,
            UseListForDataType: requestSchema.IsTypeArray(),
            IsNullable: false,
            IsRequired: true,
            AdditionalValidationAttributes: new List<ValidationAttribute>(),
            DefaultValueInitializer: null));
    }

    private static ParameterLocationType ConvertToParameterLocationType(
        ParameterLocation? openApiParameterLocation)
        => openApiParameterLocation switch
        {
            ParameterLocation.Query => ParameterLocationType.Query,
            ParameterLocation.Header => ParameterLocationType.Header,
            ParameterLocation.Path => ParameterLocationType.Route,
            ParameterLocation.Cookie => ParameterLocationType.Cookie,
            null => ParameterLocationType.None,
            _ => throw new SwitchCaseDefaultException(openApiParameterLocation),
        };

    private static IList<ValidationAttribute> GetAdditionalValidationAttributes(
        OpenApiParameter openApiParameter)
    {
        var validationAttributes = new List<ValidationAttribute>();

        AppendAdditionalValidationAttributesFromSchemaFormatIfRequired(validationAttributes, openApiParameter.Schema);
        AppendAdditionalValidationAttributesForMinMaxIfRequired(validationAttributes, openApiParameter.Schema);
        AppendAdditionalValidationAttributesForPatternIfRequired(validationAttributes, openApiParameter.Schema);

        return validationAttributes;
    }

    private static void AppendAdditionalValidationAttributesFromSchemaFormatIfRequired(
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

    private static void AppendAdditionalValidationAttributesForMinMaxIfRequired(
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

                        validationAttributes.Add(new RangeAttribute(min, max));
                    }

                    break;
                case OpenApiDataTypeConstants.Integer:
                    if (schema.HasFormatType() && schema.IsFormatTypeInt64())
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

                        validationAttributes.Add(new RangeAttribute(min, max));
                    }

                    break;
            }
        }
    }

    private static void AppendAdditionalValidationAttributesForPatternIfRequired(
        ICollection<ValidationAttribute> validationAttributes,
        OpenApiSchema schema)
    {
        if (schema.Type == OpenApiDataTypeConstants.String &&
            schema.Pattern is not null)
        {
            validationAttributes.Add(new RegularExpressionAttribute(schema.Pattern));
        }
    }
}
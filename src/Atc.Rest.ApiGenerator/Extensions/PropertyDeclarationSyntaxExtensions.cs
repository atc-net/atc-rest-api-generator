// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace
namespace Atc.CodeAnalysis.CSharp;

internal static class PropertyDeclarationSyntaxExtensions
{
    public static PropertyDeclarationSyntax AddFromHeaderAttribute(
        this PropertyDeclarationSyntax propertyDeclaration,
        string nameProperty,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(propertyDeclaration);
        ArgumentNullException.ThrowIfNull(nameProperty);

        return propertyDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.CreateWithOneItemWithOneArgumentWithNameEquals(
                    nameof(FromHeaderAttribute),
                    nameof(FromHeaderAttribute.Name),
                    nameProperty))
            .AddValidationAttributeFromSchemaFormatIfRequired(schema);
    }

    public static PropertyDeclarationSyntax AddFromRouteAttribute(
        this PropertyDeclarationSyntax propertyDeclaration,
        string nameProperty,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(propertyDeclaration);
        ArgumentNullException.ThrowIfNull(nameProperty);

        return propertyDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.CreateWithOneItemWithOneArgumentWithNameEquals(
                    nameof(FromRouteAttribute),
                    nameof(FromRouteAttribute.Name),
                    nameProperty))
            .AddValidationAttributeFromSchemaFormatIfRequired(schema);
    }

    public static PropertyDeclarationSyntax AddFromQueryAttribute(
        this PropertyDeclarationSyntax propertyDeclaration,
        string nameProperty,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(propertyDeclaration);
        ArgumentNullException.ThrowIfNull(nameProperty);

        return propertyDeclaration.AddAttributeLists(
                SyntaxAttributeListFactory.CreateWithOneItemWithOneArgumentWithNameEquals(
                    nameof(FromQueryAttribute),
                    nameof(FromQueryAttribute.Name),
                    nameProperty))
            .AddValidationAttributeFromSchemaFormatIfRequired(schema);
    }

    public static PropertyDeclarationSyntax AddFromBodyAttribute(
        this PropertyDeclarationSyntax propertyDeclaration)
    {
        ArgumentNullException.ThrowIfNull(propertyDeclaration);

        return propertyDeclaration.AddAttributeLists(SyntaxAttributeListFactory.Create(nameof(FromBodyAttribute)));
    }

    public static PropertyDeclarationSyntax AddFromFormAttribute(
        this PropertyDeclarationSyntax propertyDeclaration)
    {
        ArgumentNullException.ThrowIfNull(propertyDeclaration);

        return propertyDeclaration.AddAttributeLists(SyntaxAttributeListFactory.Create(nameof(FromFormAttribute)));
    }

    public static PropertyDeclarationSyntax AddValidationAttribute(
        this PropertyDeclarationSyntax propertyDeclaration,
        ValidationAttribute validationAttribute)
    {
        ArgumentNullException.ThrowIfNull(propertyDeclaration);
        ArgumentNullException.ThrowIfNull(validationAttribute);

        return propertyDeclaration.AddAttributeLists(
            SyntaxFactory.AttributeList(
                SyntaxFactory.SingletonSeparatedList(
                    SyntaxAttributeFactory.CreateFromValidationAttribute(validationAttribute))));
    }

    public static PropertyDeclarationSyntax AddValidationAttributeFromSchemaFormatIfRequired(
        this PropertyDeclarationSyntax propertyDeclaration,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (!string.IsNullOrEmpty(schema.Format))
        {
            return schema.Format.ToLower(CultureInfo.CurrentCulture) switch
            {
                OpenApiFormatTypeConstants.Uuid => propertyDeclaration,

                OpenApiFormatTypeConstants.Date => propertyDeclaration,
                OpenApiFormatTypeConstants.Time => propertyDeclaration,
                OpenApiFormatTypeConstants.Timestamp => propertyDeclaration,
                OpenApiFormatTypeConstants.DateTime => propertyDeclaration,

                OpenApiFormatTypeConstants.Byte => propertyDeclaration,
                OpenApiFormatTypeConstants.Binary => propertyDeclaration,
                OpenApiFormatTypeConstants.Int32 => propertyDeclaration,
                OpenApiFormatTypeConstants.Int64 => propertyDeclaration,

                OpenApiFormatTypeConstants.Float => propertyDeclaration,
                OpenApiFormatTypeConstants.Double => propertyDeclaration,

                OpenApiFormatTypeConstants.Email => propertyDeclaration.AddValidationAttributeEmail(schema),
                OpenApiFormatTypeConstants.Uri => propertyDeclaration.AddValidationAttribute(new UriAttribute()),

                _ => throw new NotImplementedException($"Schema Format '{schema.Format}' must be implemented.")
            };
        }

        return propertyDeclaration;
    }

    public static PropertyDeclarationSyntax AddValidationAttributeEmail(
        this PropertyDeclarationSyntax propertyDeclaration,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        propertyDeclaration = propertyDeclaration.AddValidationAttribute(new EmailAddressAttribute());
        if (!string.IsNullOrEmpty(schema.Pattern))
        {
            propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RegularExpressionAttribute(schema.Pattern));
        }

        return propertyDeclaration;
    }

    public static PropertyDeclarationSyntax AddValidationAttributeForMinMaxIfRequired(
        this PropertyDeclarationSyntax propertyDeclaration,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (schema.Type == OpenApiDataTypeConstants.String &&
            schema.MinLength is null &&
            schema.MaxLength is not null)
        {
            propertyDeclaration = propertyDeclaration.AddValidationAttribute(new StringLengthAttribute(schema.MaxLength.Value));
        }
        else
        {
            if (schema.MinLength is > 0)
            {
                propertyDeclaration = propertyDeclaration.AddValidationAttribute(new MinLengthAttribute(schema.MinLength.Value));
            }

            if (schema.MaxLength is > 0)
            {
                propertyDeclaration = propertyDeclaration.AddValidationAttribute(new MaxLengthAttribute(schema.MaxLength.Value));
            }

            if (schema.Minimum == null && schema.Maximum == null)
            {
                return propertyDeclaration;
            }

            propertyDeclaration = schema.Type switch
            {
                OpenApiDataTypeConstants.Number when !schema.HasFormatType() => RangeAttributeDouble(propertyDeclaration, schema),
                OpenApiDataTypeConstants.Integer when schema.HasFormatType() && schema.IsFormatTypeInt64() => RangeAttributeLong(propertyDeclaration, schema),
                _ => RangeAttributeInt(propertyDeclaration, schema)
            };
        }

        return propertyDeclaration;
    }

    public static PropertyDeclarationSyntax AddValidationAttributeForPatternIfRequired(
        this PropertyDeclarationSyntax propertyDeclaration,
        OpenApiSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        if (schema.Type == OpenApiDataTypeConstants.String &&
            schema.Pattern is not null)
        {
            propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RegularExpressionAttribute(schema.Pattern));
        }

        return propertyDeclaration;
    }

    private static PropertyDeclarationSyntax RangeAttributeInt(
        PropertyDeclarationSyntax propertyDeclaration,
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

        propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RangeAttribute(min, max));
        return propertyDeclaration;
    }

    private static PropertyDeclarationSyntax RangeAttributeLong(
        PropertyDeclarationSyntax propertyDeclaration,
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

        propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RangeAttribute(min, max));
        return propertyDeclaration;
    }

    private static PropertyDeclarationSyntax RangeAttributeDouble(
        PropertyDeclarationSyntax propertyDeclaration,
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

        propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RangeAttribute(min, max));
        return propertyDeclaration;
    }
}
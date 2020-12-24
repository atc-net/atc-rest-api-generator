using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Atc.CodeAnalysis.CSharp;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxFactories
{
    internal static class SyntaxPropertyDeclarationFactory
    {
        public static PropertyDeclarationSyntax CreateAuto(
            ParameterLocation? parameterLocation,
            bool isNullable,
            bool isRequired,
            string dataType,
            string propertyName,
            bool useNullableReferenceTypes,
            IOpenApiAny? initializer)
        {
            if (useNullableReferenceTypes && (isNullable || parameterLocation == ParameterLocation.Query) && !isRequired)
            {
                dataType += "?";
            }

            var propertyDeclaration = CreateAuto(dataType, propertyName);

            if (initializer != null)
            {
                propertyDeclaration = initializer switch
                {
                    OpenApiInteger apiInteger => propertyDeclaration.WithInitializer(
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(
                                SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(apiInteger!.Value))))
                        .WithSemicolonToken(SyntaxTokenFactory.Semicolon()),
                    OpenApiString apiString => propertyDeclaration.WithInitializer(
                            SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(
                                SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(apiString!.Value))))
                        .WithSemicolonToken(SyntaxTokenFactory.Semicolon()),
                    OpenApiBoolean apiBoolean when apiBoolean.Value => propertyDeclaration.WithInitializer(
                        SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))
                            .WithSemicolonToken(SyntaxTokenFactory.Semicolon()),
                    OpenApiBoolean apiBoolean when !apiBoolean.Value => propertyDeclaration.WithInitializer(
                        SyntaxFactory.EqualsValueClause(SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)))
                            .WithSemicolonToken(SyntaxTokenFactory.Semicolon()),
                    _ => throw new NotImplementedException("Property initializer: " + initializer.GetType())
                };
            }

            return propertyDeclaration;
        }

        public static PropertyDeclarationSyntax CreateAuto(
            KeyValuePair<string, OpenApiSchema> schema,
            ISet<string> requiredProperties,
            bool useNullableReferenceTypes)
        {
            if (requiredProperties == null)
            {
                throw new ArgumentNullException(nameof(requiredProperties));
            }

            var isNullable = schema.Value.Nullable;
            var isRequired = requiredProperties.Contains(schema.Key);

            var propertyDeclaration = schema.Value.Type == OpenApiDataTypeConstants.Array
                ? CreateListAuto(
                    schema.Value.Items.GetDataType(),
                    schema.Key.EnsureFirstCharacterToUpper())
                : CreateAuto(
                    null,
                    isNullable,
                    isRequired,
                    schema.Value.GetDataType(),
                    schema.Key.EnsureFirstCharacterToUpper(),
                    useNullableReferenceTypes,
                    schema.Value.Default);

            if (isRequired)
            {
                propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RequiredAttribute());
            }

            propertyDeclaration = propertyDeclaration.AddValidationAttributeFromSchemaFormatIfRequired(schema.Value);

            propertyDeclaration = propertyDeclaration.AddValidationAttributeForMinMaxIfRequired(schema.Value);

            return propertyDeclaration;
        }

        public static PropertyDeclarationSyntax CreateAuto(OpenApiParameter parameter, bool useNullableReferenceTypes)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameter.In == ParameterLocation.Path)
            {
                useNullableReferenceTypes = false;
            }
            else if (parameter.Schema.Default != null &&
                     (parameter.In == ParameterLocation.Query ||
                      parameter.In == ParameterLocation.Header))
            {
                useNullableReferenceTypes = false;
            }

            var propertyDeclaration = CreateAuto(
                parameter.In,
                parameter.Schema.Nullable,
                parameter.Required,
                parameter.Schema.GetDataType(),
                parameter.Name.EnsureFirstCharacterToUpper(),
                useNullableReferenceTypes,
                parameter.Schema.Default);

            propertyDeclaration = parameter.In switch
            {
                ParameterLocation.Header => propertyDeclaration.AddFromHeaderAttribute(parameter.Name, parameter.Schema),
                ParameterLocation.Path => propertyDeclaration.AddFromRouteAttribute(parameter.Name, parameter.Schema),
                ParameterLocation.Query => propertyDeclaration.AddFromQueryAttribute(parameter.Name, parameter.Schema),
                _ => throw new NotImplementedException("ParameterLocation: " + nameof(ParameterLocation) + " " + parameter.In)
            };

            if (parameter.Required)
            {
                propertyDeclaration = propertyDeclaration.AddValidationAttribute(new RequiredAttribute());
            }

            return propertyDeclaration;
        }

        public static PropertyDeclarationSyntax CreateAuto(string dataType, string propertyName)
        {
            var propertyDeclaration = SyntaxFactory
                .PropertyDeclaration(SyntaxFactory.ParseTypeName(dataType), propertyName)
                .AddModifiers(SyntaxTokenFactory.PublicKeyword())
                .AddAccessorListAccessors(
                    SyntaxAccessorDeclarationFactory.Get(),
                    SyntaxAccessorDeclarationFactory.Set());

            return propertyDeclaration;
        }

        public static PropertyDeclarationSyntax CreateListAuto(string dataType, string propertyName, bool initializeList = true)
        {
            var propertyDeclaration = SyntaxFactory.PropertyDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(Microsoft.OpenApi.Models.NameConstants.List))
                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(dataType)),
                    SyntaxFactory.Identifier(propertyName))
                .AddModifiers(SyntaxTokenFactory.PublicKeyword())
                .WithAccessorList(
                    SyntaxFactory.AccessorList(
                        SyntaxFactory.List(
                            new[]
                            {
                                SyntaxAccessorDeclarationFactory.Get(),
                                SyntaxAccessorDeclarationFactory.Set(),
                            })));

            if (initializeList)
            {
                propertyDeclaration = propertyDeclaration.WithInitializer(
                        SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.ObjectCreationExpression(
                                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(Microsoft.OpenApi.Models.NameConstants.List))
                                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(dataType)))
                                .WithArgumentList(SyntaxFactory.ArgumentList())))
                    .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
            }

            return propertyDeclaration;
        }
    }
}
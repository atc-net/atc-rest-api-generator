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
            switch (useNullableReferenceTypes)
            {
                case true when !isRequired && (isNullable || parameterLocation == ParameterLocation.Query):
                case true when isRequired && isNullable:
                    dataType += "?";
                    break;
            }

            var propertyDeclaration = CreateAuto(dataType, propertyName);
            if (initializer == null)
            {
                return propertyDeclaration;
            }

            switch (initializer)
            {
                case OpenApiInteger apiInteger:
                    propertyDeclaration = propertyDeclaration.WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                SyntaxFactory.LiteralExpression(
                                    SyntaxKind.NumericLiteralExpression,
                                    SyntaxFactory.Literal(apiInteger!.Value))))
                        .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
                    break;
                case OpenApiString apiString:
                    var expressionSyntax = string.IsNullOrEmpty(apiString!.Value)
                        ? (ExpressionSyntax)SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword()),
                            SyntaxFactory.IdentifierName("Empty"))
                        : SyntaxFactory.LiteralExpression(
                            SyntaxKind.StringLiteralExpression,
                            SyntaxFactory.Literal(apiString!.Value));

                    propertyDeclaration = propertyDeclaration.WithInitializer(
                            SyntaxFactory.EqualsValueClause(expressionSyntax))
                        .WithSemicolonToken(SyntaxTokenFactory.Semicolon());

                    break;
                case OpenApiBoolean apiBoolean when apiBoolean.Value:
                    propertyDeclaration = propertyDeclaration.WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)))
                        .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
                    break;
                case OpenApiBoolean apiBoolean when !apiBoolean.Value:
                    propertyDeclaration = propertyDeclaration.WithInitializer(
                            SyntaxFactory.EqualsValueClause(
                                SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression)))
                        .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
                    break;
                default:
                    throw new NotImplementedException("Property initializer: " + initializer.GetType());
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

            var propertyDeclaration = schema.Value.IsTypeArray()
                ? CreateListAuto(
                    schema.Value.Items.GetDataType(),
                    schema.Key.EnsureFirstCharacterToUpper(),
                    !isRequired)
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

            propertyDeclaration = propertyDeclaration.AddValidationAttributeForPatternIfRequired(schema.Value);

            return propertyDeclaration;
        }

        public static PropertyDeclarationSyntax CreateAuto(OpenApiParameter parameter, bool useNullableReferenceTypes, bool forClient)
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

            var propertyDeclaration = parameter.Schema.IsTypeArray()
                ? CreateListAuto(
                    parameter.Schema.Items.GetDataType(),
                    parameter.Name.PascalCase(removeSeparators: true))
                : CreateAuto(
                    parameter.In,
                    parameter.Schema.Nullable,
                    parameter.Required,
                    parameter.Schema.GetDataType(),
                    parameter.Name.PascalCase(removeSeparators: true),
                    useNullableReferenceTypes,
                    parameter.Schema.Default);

            if (!forClient)
            {
                propertyDeclaration = parameter.In switch
                {
                    ParameterLocation.Header => propertyDeclaration.AddFromHeaderAttribute(parameter.Name, parameter.Schema),
                    ParameterLocation.Path => propertyDeclaration.AddFromRouteAttribute(parameter.Name, parameter.Schema),
                    ParameterLocation.Query => propertyDeclaration.AddFromQueryAttribute(parameter.Name, parameter.Schema),
                    _ => throw new NotImplementedException("ParameterLocation: " + nameof(ParameterLocation) + " " + parameter.In)
                };
            }

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
                            .WithArgumentList(
                                SyntaxFactory.ArgumentList())))
                    .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
            }

            return propertyDeclaration;
        }
    }
}
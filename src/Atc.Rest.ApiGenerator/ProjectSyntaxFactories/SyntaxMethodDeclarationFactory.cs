using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Models;

// ReSharper disable InvertIf
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.ProjectSyntaxFactories
{
    internal static class SyntaxMethodDeclarationFactory
    {
        public static MethodDeclarationSyntax CreateInterfaceMethod(string parameterTypeName, string resultTypeName, bool hasParameters)
        {
            if (parameterTypeName == null)
            {
                throw new ArgumentNullException(nameof(parameterTypeName));
            }

            if (resultTypeName == null)
            {
                throw new ArgumentNullException(nameof(resultTypeName));
            }

            var arguments = hasParameters
                ? new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(parameterTypeName, "parameters"),
                    SyntaxTokenFactory.Comma(),
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                }
                : new SyntaxNodeOrToken[]
                {
                    SyntaxParameterFactory.Create(nameof(CancellationToken), nameof(CancellationToken).EnsureFirstCharacterToLower())
                        .WithDefault(SyntaxFactory.EqualsValueClause(
                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, SyntaxTokenFactory.DefaultKeyword()))),
                };

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(nameof(Task)))
                        .WithTypeArgumentList(SyntaxTypeArgumentListFactory.CreateWithOneItem(resultTypeName)),
                    SyntaxFactory.Identifier("ExecuteAsync"))
                .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>(arguments)))
                .WithSemicolonToken(SyntaxTokenFactory.Semicolon());
        }

        public static MemberDeclarationSyntax? CreateToStringMethod(IDictionary<string, OpenApiSchema> apiSchemaProperties)
        {
            if (apiSchemaProperties == null)
            {
                throw new ArgumentNullException(nameof(apiSchemaProperties));
            }

            var content = new List<InterpolatedStringContentSyntax>();
            if (apiSchemaProperties.Count > 0)
            {
                var lastKey = apiSchemaProperties.Keys.Last();
                foreach (var schema in apiSchemaProperties)
                {
                    var name = schema.Key.EnsureFirstCharacterToUpper();
                    var hasAnyProperties = schema.Value.HasAnyProperties();

                    if (!hasAnyProperties)
                    {
                        content.Add(SyntaxInterpolatedFactory.CreateNameOf(name));
                        content.Add(SyntaxInterpolatedFactory.StringTextColon());
                        content.Add(SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName(name)));
                    }
                    else
                    {
                        content.Add(SyntaxInterpolatedFactory.CreateNameOf(name));
                        content.Add(SyntaxInterpolatedFactory.StringTextColonAndParenthesesStart());
                        content.Add(SyntaxFactory.Interpolation(SyntaxFactory.IdentifierName(name)));
                        content.Add(SyntaxInterpolatedFactory.StringTextParenthesesEnd());
                    }

                    if (schema.Key != lastKey)
                    {
                        content.Add(SyntaxInterpolatedFactory.StringTextComma());
                    }
                }
            }

            if (content.Count == 0)
            {
                return null;
            }

            var codeBody = SyntaxFactory.Block(
                SyntaxFactory.SingletonList<StatementSyntax>(
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.InterpolatedStringExpression(
                                SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken))
                            .WithContents(SyntaxFactory.List(content)))));

            return SyntaxFactory.MethodDeclaration(
                    SyntaxFactory.PredefinedType(SyntaxTokenFactory.StringKeyword()),
                    SyntaxFactory.Identifier("ToString"))
                .WithModifiers(SyntaxTokenListFactory.PublicOverrideKeyword())
                .WithBody(codeBody);
        }

        public static MemberDeclarationSyntax? CreateToStringMethod(
            IList<OpenApiParameter>? globalPathParameters,
            IList<OpenApiParameter>? apiParameters,
            OpenApiRequestBody? apiRequestBody)
        {
            var dictionary = new Dictionary<string, OpenApiSchema>();
            if (globalPathParameters != null)
            {
                foreach (var pathParameter in globalPathParameters)
                {
                    dictionary.Add(pathParameter.Name, pathParameter.Schema);
                }
            }

            if (apiParameters != null)
            {
                foreach (var apiParameter in apiParameters)
                {
                    dictionary.Add(apiParameter.Name, apiParameter.Schema);
                }
            }

            var schema = apiRequestBody?.Content?.GetSchema();
            if (schema != null && !dictionary.ContainsKey(NameConstants.Request))
            {
                dictionary.Add(NameConstants.Request, schema);
            }

            return CreateToStringMethod(dictionary);
        }
    }
}
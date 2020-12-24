using System;
using System.Collections.Generic;
using System.Linq;
using Atc.CodeAnalysis.CSharp.Factories;
using Atc.CodeAnalysis.CSharp.SyntaxFactories;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.SyntaxFactories
{
    internal static class SyntaxEnumFactory
    {
        public static EnumDeclarationSyntax Create(string title, OpenApiSchema apiSchema)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (apiSchema == null)
            {
                throw new ArgumentNullException(nameof(apiSchema));
            }

            return Create(title, apiSchema.Enum);
        }

        public static EnumDeclarationSyntax Create(string title, IList<IOpenApiAny> apiSchemaEnums)
        {
            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (apiSchemaEnums == null)
            {
                throw new ArgumentNullException(nameof(apiSchemaEnums));
            }

            // Create an enum
            var enumDeclaration = SyntaxFactory.EnumDeclaration(title)
                .AddModifiers(SyntaxTokenFactory.PublicKeyword());

            // Find values to the enum
            var containTypeName = false;
            var lines = new List<string>();
            var intValues = new List<int>();
            foreach (var item in apiSchemaEnums)
            {
                if (!(item is OpenApiString openApiString))
                {
                    continue;
                }

                lines.Add(openApiString.Value.Trim());

                if (!openApiString.Value.Contains("=", StringComparison.Ordinal))
                {
                    continue;
                }

                var sa = openApiString.Value.Split('=', StringSplitOptions.RemoveEmptyEntries);
                var s = sa.Last().Trim();
                if (int.TryParse(s, out int val))
                {
                    intValues.Add(val);
                }
            }

            // Add values to the enum
            foreach (var line in lines)
            {
                enumDeclaration = enumDeclaration.AddMembers(SyntaxFactory.EnumMemberDeclaration(line));

                var sa = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (sa.Length > 0 && sa.First().Equals("Object", StringComparison.Ordinal))
                {
                    containTypeName = true;
                }
            }

            // Add SuppressMessageAttribute
            if (containTypeName)
            {
                enumDeclaration = enumDeclaration
                    .AddSuppressMessageAttribute(SuppressMessageAttributeFactory.Create(1720, null));
            }

            // Add FlagAttribute
            if (intValues.Count > 0)
            {
                bool isAllValidBinarySequence = intValues
                    .Where(x => x != 0)
                    .All(x => x.IsBinarySequence());
                if (isAllValidBinarySequence)
                {
                    enumDeclaration = enumDeclaration.AddFlagAttribute();
                }
            }

            return enumDeclaration;
        }
    }
}
using System;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atc.Rest.ApiGenerator.ProjectSyntaxFactories
{
    internal static class SyntaxProjectFactory
    {
        public static NamespaceDeclarationSyntax CreateNamespace(BaseProjectOptions baseProjectOptions, bool withAutoGen = true)
        {
            if (baseProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(baseProjectOptions));
            }

            if (withAutoGen)
            {
                return SyntaxNamespaceDeclarationFactory.Create(
                    baseProjectOptions.ToolNameAndVersion,
                    baseProjectOptions.ProjectName);
            }

            return SyntaxNamespaceDeclarationFactory.Create(
                baseProjectOptions.ProjectName);
        }

        public static NamespaceDeclarationSyntax CreateNamespace(BaseProjectOptions baseProjectOptions, string namespacePart, bool withAutoGen = true)
        {
            if (baseProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(baseProjectOptions));
            }

            if (namespacePart == null)
            {
                throw new ArgumentNullException(nameof(namespacePart));
            }

            if (withAutoGen)
            {
                return SyntaxNamespaceDeclarationFactory.Create(
                    baseProjectOptions.ToolNameAndVersion,
                    $"{baseProjectOptions.ProjectName}.{namespacePart}");
            }

            return SyntaxNamespaceDeclarationFactory.Create(
                $"{baseProjectOptions.ProjectName}.{namespacePart}");
        }

        public static NamespaceDeclarationSyntax CreateNamespace(BaseProjectOptions baseProjectOptions, string namespacePart, string focusOnSegmentName, bool withAutoGen = true)
        {
            if (baseProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(baseProjectOptions));
            }

            if (namespacePart == null)
            {
                throw new ArgumentNullException(nameof(namespacePart));
            }

            if (focusOnSegmentName == null)
            {
                throw new ArgumentNullException(nameof(focusOnSegmentName));
            }

            if (withAutoGen)
            {
                return SyntaxNamespaceDeclarationFactory.Create(
                    baseProjectOptions.ToolNameAndVersion,
                    $"{baseProjectOptions.ProjectName}.{namespacePart}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}");
            }

            return SyntaxNamespaceDeclarationFactory.Create(
                $"{baseProjectOptions.ProjectName}.{namespacePart}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}");
        }
    }
}
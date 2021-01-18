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

            var fullNamespace = string.IsNullOrEmpty(baseProjectOptions.ClientFolderName)
                ? $"{baseProjectOptions.ProjectName}"
                : $"{baseProjectOptions.ProjectName}.{baseProjectOptions.ClientFolderName}";

            if (withAutoGen)
            {
                return SyntaxNamespaceDeclarationFactory.Create(
                    baseProjectOptions.ToolNameAndVersion,
                    fullNamespace);
            }

            return SyntaxNamespaceDeclarationFactory.Create(
                fullNamespace);
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

            var fullNamespace = string.IsNullOrEmpty(baseProjectOptions.ClientFolderName)
                ? $"{baseProjectOptions.ProjectName}.{namespacePart}"
                : $"{baseProjectOptions.ProjectName}.{baseProjectOptions.ClientFolderName}.{namespacePart}";

            if (withAutoGen)
            {
                return SyntaxNamespaceDeclarationFactory.Create(
                    baseProjectOptions.ToolNameAndVersion,
                    fullNamespace);
            }

            return SyntaxNamespaceDeclarationFactory.Create(
                fullNamespace);
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

            var fullNamespace = string.IsNullOrEmpty(baseProjectOptions.ClientFolderName)
                ? $"{baseProjectOptions.ProjectName}.{namespacePart}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}"
                : $"{baseProjectOptions.ProjectName}.{baseProjectOptions.ClientFolderName}.{namespacePart}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}";

            if (withAutoGen)
            {
                return SyntaxNamespaceDeclarationFactory.Create(
                    baseProjectOptions.ToolNameAndVersion,
                    fullNamespace);
            }

            return SyntaxNamespaceDeclarationFactory.Create(
                fullNamespace);
        }
    }
}
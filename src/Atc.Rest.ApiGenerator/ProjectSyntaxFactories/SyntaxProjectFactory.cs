namespace Atc.Rest.ApiGenerator.ProjectSyntaxFactories;

internal static class SyntaxProjectFactory
{
    public static NamespaceDeclarationSyntax CreateNamespace(
        BaseProjectOptions baseProjectOptions,
        bool withAutoGen = true)
    {
        ArgumentNullException.ThrowIfNull(baseProjectOptions);

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

    public static NamespaceDeclarationSyntax CreateNamespace(
        BaseProjectOptions baseProjectOptions,
        string namespacePart,
        bool withAutoGen = true)
    {
        ArgumentNullException.ThrowIfNull(baseProjectOptions);
        ArgumentNullException.ThrowIfNull(namespacePart);

        string fullNamespace;
        if (baseProjectOptions.IsForClient)
        {
            fullNamespace = string.IsNullOrEmpty(baseProjectOptions.ClientFolderName)
                ? $"{baseProjectOptions.ProjectName}.{namespacePart}"
                : $"{baseProjectOptions.ProjectName}.{baseProjectOptions.ClientFolderName}.{namespacePart}";
        }
        else
        {
            fullNamespace = $"{baseProjectOptions.ProjectName}.{namespacePart}";
        }

        if (withAutoGen)
        {
            return SyntaxNamespaceDeclarationFactory.Create(
                baseProjectOptions.ToolNameAndVersion,
                fullNamespace);
        }

        return SyntaxNamespaceDeclarationFactory.Create(fullNamespace);
    }

    public static NamespaceDeclarationSyntax CreateNamespace(
        BaseProjectOptions baseProjectOptions,
        string namespacePart,
        string focusOnSegmentName,
        bool withAutoGen = true)
    {
        ArgumentNullException.ThrowIfNull(baseProjectOptions);
        ArgumentNullException.ThrowIfNull(namespacePart);
        ArgumentNullException.ThrowIfNull(focusOnSegmentName);

        string fullNamespace;
        if (baseProjectOptions.IsForClient)
        {
            fullNamespace = string.IsNullOrEmpty(baseProjectOptions.ClientFolderName)
                ? $"{baseProjectOptions.ProjectName}.{namespacePart}"
                : $"{baseProjectOptions.ProjectName}.{baseProjectOptions.ClientFolderName}.{namespacePart}";
        }
        else
        {
            fullNamespace = $"{baseProjectOptions.ProjectName}.{namespacePart}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}";
        }

        if (withAutoGen)
        {
            return SyntaxNamespaceDeclarationFactory.Create(
                baseProjectOptions.ToolNameAndVersion,
                fullNamespace);
        }

        return SyntaxNamespaceDeclarationFactory.Create(fullNamespace);
    }
}
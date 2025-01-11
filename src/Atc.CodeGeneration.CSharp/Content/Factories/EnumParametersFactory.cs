namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class EnumParametersFactory
{
    public static EnumParameters Create(
        string headerContent,
        string @namespace,
        CodeDocumentationTags documentationTags,
        IList<AttributeParameters> attributes,
        string enumTypeName,
        string[] enumNames)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: documentationTags,
            Attributes: attributes,
            DeclarationModifier: DeclarationModifiers.Public,
            EnumTypeName: enumTypeName,
            UseFlags: false,
            EnumValuesParametersFactory.Create(enumNames));

    public static EnumParameters Create(
        string headerContent,
        string @namespace,
        CodeDocumentationTags documentationTags,
        IList<AttributeParameters> attributes,
        string enumTypeName,
        IDictionary<string, int?> enumNameValues)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: documentationTags,
            Attributes: attributes,
            DeclarationModifier: DeclarationModifiers.Public,
            EnumTypeName: enumTypeName,
            UseFlags: DetermineIfFlagsAttributeShouldBeUsed(enumNameValues),
            EnumValuesParametersFactory.Create(enumNameValues));

    private static bool DetermineIfFlagsAttributeShouldBeUsed(
        IDictionary<string, int?> enumNameValues)
    {
        if (enumNameValues.All(x => !x.Value.HasValue))
        {
            return false;
        }

        return enumNameValues
            .Where(x => x.Value.HasValue && x.Value.Value != 0)
            .All(x => x.Value.HasValue && x.Value.Value.IsBinarySequence());
    }
}
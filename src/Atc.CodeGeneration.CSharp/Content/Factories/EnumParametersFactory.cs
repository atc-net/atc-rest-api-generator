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
            AccessModifier: AccessModifiers.Public,
            EnumTypeName: enumTypeName,
            UseFlags: false,
            EnumValuesParametersFactory.Create(enumNames));

    public static EnumParameters Create(
        string headerContent,
        string @namespace,
        CodeDocumentationTags documentationTags,
        IList<AttributeParameters> attributes,
        string enumTypeName,
        IDictionary<string, int> enumNameValues)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: documentationTags,
            Attributes: attributes,
            AccessModifier: AccessModifiers.Public,
            EnumTypeName: enumTypeName,
            UseFlags: AnalyzeForUseFlagAttribute(enumNameValues),
            EnumValuesParametersFactory.Create(enumNameValues));

    private static bool AnalyzeForUseFlagAttribute(
        IDictionary<string, int> enumNameValues)
        => enumNameValues
            .Where(x => x.Value != 0)
            .All(x => x.Value.IsBinarySequence());
}
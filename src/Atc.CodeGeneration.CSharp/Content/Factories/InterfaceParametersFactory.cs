namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class InterfaceParametersFactory
{
    public static InterfaceParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters attribute,
        string interfaceTypeName)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: null,
            Attributes: new List<AttributeParameters> { attribute },
            DeclarationModifiers.Public,
            InterfaceTypeName: interfaceTypeName,
            InheritedInterfaceTypeName: null,
            Properties: null,
            Methods: null);

    public static InterfaceParameters Create(
        string headerContent,
        string @namespace,
        List<AttributeParameters> attributes,
        string interfaceTypeName)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: null,
            Attributes: attributes,
            DeclarationModifiers.PublicInterface,
            InterfaceTypeName: interfaceTypeName,
            InheritedInterfaceTypeName: null,
            Properties: null,
            Methods: null);
}
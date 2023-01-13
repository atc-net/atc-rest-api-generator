namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class ClassParametersFactory
{
    public static ClassParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters attribute,
        string classTypeName)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: null,
            Attributes: new List<AttributeParameters> { attribute },
            AccessModifiers.Public,
            ClassTypeName: classTypeName,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: null,
            GenerateToStringMethode: false);
}
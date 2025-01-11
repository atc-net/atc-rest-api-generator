namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class ClassParametersFactory
{
    public static ClassParameters Create(
        string headerContent,
        string @namespace,
        AttributeParameters attribute,
        string classTypeName,
        bool usePartialClass = false)
        => new(
            HeaderContent: headerContent,
            Namespace: @namespace,
            DocumentationTags: null,
            Attributes: new List<AttributeParameters> { attribute },
            usePartialClass ? DeclarationModifiers.PublicPartialClass : DeclarationModifiers.PublicClass,
            ClassTypeName: classTypeName,
            GenericTypeName: null,
            InheritedClassTypeName: null,
            InheritedGenericClassTypeName: null,
            InheritedInterfaceTypeName: null,
            Constructors: null,
            Properties: null,
            Methods: null,
            GenerateToStringMethod: false);
}
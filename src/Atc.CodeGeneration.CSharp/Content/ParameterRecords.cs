namespace Atc.CodeGeneration.CSharp.Content;

public record BaseParameters(
    string? HeaderContent,
    string Namespace,
    CodeDocumentationTags? DocumentationTags,
    AccessModifiers AccessModifier,
    string TypeName);

public record InterfaceParameters(
    string? HeaderContent,
    string Namespace,
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string InterfaceTypeName,
    string? InheritedInterfaceTypeName,
    IList<PropertyParameters>? Properties,
    IList<MethodParameters>? Methods)
    : BaseParameters(
        HeaderContent,
        Namespace,
        DocumentationTags,
        AccessModifier,
        InterfaceTypeName);

public record ClassParameters(
    string? HeaderContent,
    string Namespace,
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string ClassTypeName,
    string? InheritedClassTypeName,
    string? InheritedGenericClassTypeName,
    string? InheritedInterfaceTypeName,
    IList<PropertyParameters>? Properties,
    IList<MethodParameters>? Methods)
    : BaseParameters(
        HeaderContent,
        Namespace,
        DocumentationTags,
        AccessModifier,
        ClassTypeName);

public record EnumParameters(
    string? HeaderContent,
    string Namespace,
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string EnumTypeName,
    bool UseFlag,
    IList<EnumValueParameters> Values)
    : BaseParameters(
        HeaderContent,
        Namespace,
        DocumentationTags,
        AccessModifier,
        EnumTypeName);

public record AttributeParameters(
    string Name,
    string? Content);

public record PropertyParameters(
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string? ReturnGenericTypeName,
    string ReturnTypeName,
    string Name,
    string? DefaultValue,
    bool UseAutoProperty,
    bool UseGet,
    bool UseSet,
    bool UseExpressionBody,
    string? Content);

public record MethodParameters(
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string ReturnTypeName,
    string? ReturnGenericTypeName,
    string Name,
    IList<PropertyParameters>? Parameters,
    bool UseExpressionBody,
    string? Content);

public record EnumValueParameters(
    CodeDocumentationTags? DocumentationTags,
    AttributeParameters? DescriptionAttribute,
    string Name,
    int? Value);
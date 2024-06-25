namespace Atc.CodeGeneration.CSharp.Content;

public record SolutionConfigurationParameters(
    Guid ConfigurationId);

public record SolutionProjectParameters(
    Guid ProjectTypeId,
    string Name,
    string RelativePath,
    Guid ConfigurationId);

public record SolutionFileParameters(
    Guid SolutionId,
    IList<SolutionConfigurationParameters> Configurations,
    IList<SolutionProjectParameters> Projects);

[SuppressMessage("X", "S2094: Remove this empty record, write its code or make it an interface", Justification = "OK - WIP")]
public record SolutionDotSettingsFileParameters();

public record PropertyGroupParameter(
    string Key,
    IList<KeyValuePair<string, string>>? Attributes,
    string? Value);

public record ItemGroupParameter(
    string Key,
    IList<KeyValuePair<string, string>>? Attributes,
    string? Value);

public record ProjectFileParameters(
    string ProjectSdK,
    IList<IList<PropertyGroupParameter>> PropertyGroups,
    IList<IList<ItemGroupParameter>> ItemGroups);

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
    string? GenericTypeName,
    string? InheritedClassTypeName,
    string? InheritedGenericClassTypeName,
    string? InheritedInterfaceTypeName,
    IList<ConstructorParameters>? Constructors,
    IList<PropertyParameters>? Properties,
    IList<MethodParameters>? Methods,
    bool GenerateToStringMethod)
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
    bool UseFlags,
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

public record ParameterBaseParameters(
    IList<AttributeParameters>? Attributes,
    string? GenericTypeName,
    bool IsGenericListType,
    string TypeName,
    bool IsNullableType,
    bool IsReferenceType,
    string Name,
    string? DefaultValue);

public record ConstructorParameterBaseParameters(
    string? GenericTypeName,
    string TypeName,
    bool IsNullableType,
    string Name,
    string? DefaultValue,
    bool PassToInheritedClass,
    bool CreateAsPrivateReadonlyMember,
    bool CreateAaOneLiner);

public record ConstructorParameters(
    CodeDocumentationTags? DocumentationTags,
    AccessModifiers AccessModifier,
    string? GenericTypeName,
    string TypeName,
    string? InheritedClassTypeName,
    IList<ConstructorParameterBaseParameters>? Parameters);

public record PropertyParameters(
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string? GenericTypeName,
    string TypeName,
    bool IsNullableType,
    string Name,
    string? JsonName,
    string? DefaultValue,
    bool IsReferenceType,
    bool IsGenericListType,
    bool UseAutoProperty,
    bool UseGet,
    bool UseSet,
    bool UseExpressionBody,
    string? Content)
    : ParameterBaseParameters(
        Attributes,
        GenericTypeName,
        IsGenericListType,
        TypeName,
        IsNullableType,
        IsReferenceType,
        Name,
        DefaultValue);

public record MethodParameters(
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    AccessModifiers AccessModifier,
    string? ReturnGenericTypeName,
    string? ReturnTypeName,
    string Name,
    IList<ParameterBaseParameters>? Parameters,
    bool AlwaysBreakDownParameters,
    bool UseExpressionBody,
    string? Content);

public record EnumValueParameters(
    CodeDocumentationTags? DocumentationTags,
    AttributeParameters? DescriptionAttribute,
    string Name,
    int? Value);

public record RecordsParameters(
    string? HeaderContent,
    string Namespace,
    CodeDocumentationTags? DocumentationTags,
    IList<AttributeParameters>? Attributes,
    IList<RecordParameters> Parameters);

public record RecordParameters(
    CodeDocumentationTags? DocumentationTags,
    AccessModifiers AccessModifier,
    string Name,
    IList<ParameterBaseParameters>? Parameters);
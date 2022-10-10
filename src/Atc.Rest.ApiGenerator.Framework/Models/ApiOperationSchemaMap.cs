namespace Atc.Rest.ApiGenerator.Framework.Models;

public sealed class ApiOperationSchemaMap
{
    public ApiOperationSchemaMap(
        string schemaKey,
        ApiSchemaMapLocatedAreaType locatedArea,
        string path,
        HttpOperationType httpOperation,
        string? parentSchemaKey)
    {
        SchemaKey = schemaKey;
        LocatedArea = locatedArea;
        Path = path;
        HttpOperation = httpOperation;
        ParentSchemaKey = parentSchemaKey;
        SegmentName = ExtractSegmentName(Path);
    }

    public string SchemaKey { get; }

    public ApiSchemaMapLocatedAreaType LocatedArea { get; }

    public string SegmentName { get; }

    public string Path { get; }

    public HttpOperationType HttpOperation { get; }

    public string? ParentSchemaKey { get; }

    public bool IsEnum { get; set; }

    public bool IsShared { get; set; }

    public CardinalityType Cardinality { get; set; }

    public override string ToString()
        => $"{nameof(SchemaKey)}: {SchemaKey}, {nameof(LocatedArea)}: {LocatedArea}, {nameof(SegmentName)}: {SegmentName}, {nameof(Path)}: {Path}, {nameof(HttpOperation)}: {HttpOperation}, {nameof(ParentSchemaKey)}: {ParentSchemaKey}, {nameof(IsEnum)}: {IsEnum}, {nameof(IsShared)}: {IsShared}, {nameof(Cardinality)}: {Cardinality}";

    public override int GetHashCode()
        => HashCode.Combine(SchemaKey, (int)LocatedArea, SegmentName, Path, (int)HttpOperation, ParentSchemaKey);

    public override bool Equals(
        object? obj)
        => obj is not null &&
           (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((ApiOperationSchemaMap)obj)));

    /// <summary>
    /// Compares two instances of ApiOperationSchemaMap.
    /// </summary>
    /// <remarks>
    /// We do not want to compare on IsEnum, IsShared, Cardinality!
    /// </remarks>
    private bool Equals(
        ApiOperationSchemaMap other)
        => SchemaKey == other.SchemaKey &&
           LocatedArea == other.LocatedArea &&
           SegmentName == other.SegmentName &&
           Path == other.Path &&
           HttpOperation == other.HttpOperation &&
           ParentSchemaKey == other.ParentSchemaKey;

    private static string ExtractSegmentName(
        string path)
        => path
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            .First()
            .PascalCase(removeSeparators: true);
}
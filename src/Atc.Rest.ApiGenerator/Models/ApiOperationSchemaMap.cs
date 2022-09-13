namespace Atc.Rest.ApiGenerator.Models;

public sealed class ApiOperationSchemaMap
{
    public ApiOperationSchemaMap(
        string schemaKey,
        SchemaMapLocatedAreaType locatedArea,
        string path,
        OperationType operationType,
        string? parentSchemaKey)
    {
        SchemaKey = schemaKey;
        LocatedArea = locatedArea;
        Path = path;
        OperationType = operationType;
        ParentSchemaKey = parentSchemaKey;

        SegmentName = OpenApiOperationSchemaMapHelper.GetSegmentName(Path);
    }

    public string SchemaKey { get; }

    public SchemaMapLocatedAreaType LocatedArea { get; }

    public string SegmentName { get; }

    public string Path { get; }

    public OperationType OperationType { get; }

    public string? ParentSchemaKey { get; }

    public override string ToString()
        => $"{nameof(SchemaKey)}: {SchemaKey}, {nameof(LocatedArea)}: {LocatedArea}, {nameof(SegmentName)}: {SegmentName}, {nameof(Path)}: {Path}, {nameof(OperationType)}: {OperationType}, {nameof(ParentSchemaKey)}: {ParentSchemaKey}";

    private bool Equals(
        ApiOperationSchemaMap other)
        => SchemaKey == other.SchemaKey &&
           LocatedArea == other.LocatedArea &&
           SegmentName == other.SegmentName &&
           Path == other.Path &&
           OperationType == other.OperationType &&
           ParentSchemaKey == other.ParentSchemaKey;

    public override bool Equals(
        object? obj)
        => !ReferenceEquals(null, obj) &&
           (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((ApiOperationSchemaMap)obj)));

    public override int GetHashCode()
        => HashCode.Combine(SchemaKey, (int)LocatedArea, SegmentName, Path, (int)OperationType, ParentSchemaKey);
}
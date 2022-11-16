namespace Atc.Rest.ApiGenerator.Framework.Models;

public sealed class ApiOperation
{
    public ApiOperation(
        string schemaKey,
        ApiSchemaMapLocatedAreaType locatedArea,
        string path,
        HttpOperationType httpOperation,
        string? parentSchemaKey)
    {
        Model = new ApiOperationModel
        {
            Name = schemaKey,
        };

        LocatedArea = locatedArea;
        Path = path;
        HttpOperation = httpOperation;
        ParentSchemaKey = parentSchemaKey;
        SegmentName = ExtractSegmentName(Path);
    }

    public ApiOperationModel Model { get; }

    public ApiSchemaMapLocatedAreaType LocatedArea { get; }

    public string SegmentName { get; }

    public string Path { get; }

    public HttpOperationType HttpOperation { get; }

    public string? ParentSchemaKey { get; }

    public CardinalityType Cardinality { get; set; }

    public override string ToString()
        => $"{nameof(Model)}: ({Model}), {nameof(LocatedArea)}: {LocatedArea}, {nameof(SegmentName)}: {SegmentName}, {nameof(Path)}: {Path}, {nameof(HttpOperation)}: {HttpOperation}, {nameof(ParentSchemaKey)}: {ParentSchemaKey}, {nameof(Cardinality)}: {Cardinality}";

    public override int GetHashCode()
        => HashCode.Combine(Model.Name, (int)LocatedArea, SegmentName, Path, (int)HttpOperation, ParentSchemaKey);

    public override bool Equals(
        object? obj)
        => obj is not null &&
           (ReferenceEquals(this, obj) || (obj.GetType() == GetType() && Equals((ApiOperation)obj)));

    /// <summary>
    /// Compares two instances of ApiOperation.
    /// </summary>
    /// <remarks>
    /// We do not want to compare on Cardinality!
    /// </remarks>
    private bool Equals(
        ApiOperation other)
        => Model.Name == other.Model.Name &&
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
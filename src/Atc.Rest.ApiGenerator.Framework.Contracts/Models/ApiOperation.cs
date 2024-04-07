namespace Atc.Rest.ApiGenerator.Framework.Contracts.Models;

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
        ApiGroupName = ExtractApiGroupName(Path);
    }

    public ApiOperationModel Model { get; }

    public ApiSchemaMapLocatedAreaType LocatedArea { get; }

    public string ApiGroupName { get; }

    public string Path { get; }

    public HttpOperationType HttpOperation { get; }

    public string? ParentSchemaKey { get; }

    public CardinalityType Cardinality { get; set; }

    public override string ToString()
        => $"{nameof(Model)}: ({Model}), {nameof(LocatedArea)}: {LocatedArea}, {nameof(ApiGroupName)}: {ApiGroupName}, {nameof(Path)}: {Path}, {nameof(HttpOperation)}: {HttpOperation}, {nameof(ParentSchemaKey)}: {ParentSchemaKey}, {nameof(Cardinality)}: {Cardinality}";

    public override int GetHashCode()
        => HashCode.Combine(Model.Name, (int)LocatedArea, ApiGroupName, Path, (int)HttpOperation, ParentSchemaKey);

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
           ApiGroupName == other.ApiGroupName &&
           Path == other.Path &&
           HttpOperation == other.HttpOperation &&
           ParentSchemaKey == other.ParentSchemaKey;

    private static string ExtractApiGroupName(
        string path)
        => path
            .Split('/', StringSplitOptions.RemoveEmptyEntries)
            [0]
            .PascalCase(removeSeparators: true);
}
using System;
using Atc.Rest.ApiGenerator.Helpers;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Models
{
    public sealed class ApiOperationSchemaMap
    {
        public ApiOperationSchemaMap(string schemaKey, SchemaMapLocatedAreaType locatedArea, string path, OperationType operationType, string? parentSchemaKey)
        {
            this.SchemaKey = schemaKey;
            this.LocatedArea = locatedArea;
            this.Path = path;
            this.OperationType = operationType;
            this.ParentSchemaKey = parentSchemaKey;

            this.SegmentName = OpenApiOperationSchemaMapHelper.GetSegmentName(this.Path);
        }

        public string SchemaKey { get; }

        public SchemaMapLocatedAreaType LocatedArea { get; }

        public string SegmentName { get; }

        public string Path { get; }

        public OperationType OperationType { get; }

        public string? ParentSchemaKey { get; }

        public override string ToString()
        {
            return $"{nameof(SchemaKey)}: {SchemaKey}, {nameof(LocatedArea)}: {LocatedArea}, {nameof(SegmentName)}: {SegmentName}, {nameof(Path)}: {Path}, {nameof(OperationType)}: {OperationType}, {nameof(ParentSchemaKey)}: {ParentSchemaKey}";
        }

        private bool Equals(ApiOperationSchemaMap other)
        {
            return SchemaKey == other.SchemaKey &&
                   LocatedArea == other.LocatedArea &&
                   SegmentName == other.SegmentName &&
                   Path == other.Path &&
                   OperationType == other.OperationType &&
                   ParentSchemaKey == other.ParentSchemaKey;
        }

        public override bool Equals(object? obj)
        {
            return !ReferenceEquals(null, obj) &&
                   (ReferenceEquals(this, obj) || (obj.GetType() == this.GetType() && Equals((ApiOperationSchemaMap)obj)));
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SchemaKey, (int)LocatedArea, SegmentName, Path, (int)OperationType, ParentSchemaKey);
        }
    }
}
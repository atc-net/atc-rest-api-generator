using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Rest.ApiGenerator.Extensions;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api
{
    public class SyntaxGeneratorContractModels : ISyntaxGeneratorContractModels
    {
        public SyntaxGeneratorContractModels(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            string focusOnSegmentName)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        public List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public string FocusOnSegmentName { get; }

        public List<SyntaxGeneratorContractModel> GenerateSyntaxTrees()
        {
            var list = new List<SyntaxGeneratorContractModel>();

            var apiOperationSchemaMaps = OperationSchemaMappings
                .Where(x => (x.LocatedArea == SchemaMapLocatedAreaType.Response ||
                             x.LocatedArea == SchemaMapLocatedAreaType.RequestBody) &&
                             x.SegmentName.Equals(FocusOnSegmentName, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var schemaKeys = apiOperationSchemaMaps
                .Select(x => x.SchemaKey)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            foreach (var schemaKey in schemaKeys)
            {
                var apiSchema = ApiProjectOptions.Document.Components.Schemas.First(x => x.Key.Equals(schemaKey, StringComparison.OrdinalIgnoreCase));
                if (apiSchema.Value.IsSchemaEnumOrPropertyEnum())
                {
                    var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(ApiProjectOptions, apiSchema.Key, apiSchema.Value, FocusOnSegmentName);
                    syntaxGeneratorContractModel.GenerateCode();
                    list.Add(syntaxGeneratorContractModel);
                }
                else
                {
                    var isShared = OperationSchemaMappings.IsShared(schemaKey);
                    if (isShared)
                    {
                        var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(ApiProjectOptions, apiSchema.Key, apiSchema.Value, "#");
                        syntaxGeneratorContractModel.GenerateCode();
                        list.Add(syntaxGeneratorContractModel);
                    }
                    else
                    {
                        var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(ApiProjectOptions, apiSchema.Key, apiSchema.Value, FocusOnSegmentName);
                        syntaxGeneratorContractModel.GenerateCode();
                        list.Add(syntaxGeneratorContractModel);
                    }
                }
            }

            return list;
        }
    }
}
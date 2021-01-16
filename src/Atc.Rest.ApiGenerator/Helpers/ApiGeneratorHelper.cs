using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class ApiGeneratorHelper
    {
        public static void CollectMissingContractModelFromOperationSchemaMappings(
            ApiProjectOptions projectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            List<SyntaxGeneratorContractModel> sgContractModels)
        {
            if (projectOptions == null)
            {
                throw new ArgumentNullException(nameof(projectOptions));
            }

            if (operationSchemaMappings == null)
            {
                throw new ArgumentNullException(nameof(operationSchemaMappings));
            }

            if (sgContractModels == null)
            {
                throw new ArgumentNullException(nameof(sgContractModels));
            }

            var missingOperationSchemaMappings = new List<ApiOperationSchemaMap>();
            foreach (var map in operationSchemaMappings)
            {
                if (sgContractModels.FirstOrDefault(x => x.ApiSchemaKey.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)) == null)
                {
                    missingOperationSchemaMappings.Add(map);
                }
            }

            foreach (var map in missingOperationSchemaMappings)
            {
                if (missingOperationSchemaMappings.Count(x => x.SchemaKey.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)) > 1)
                {
                    throw new NotImplementedException($"SchemaKey: {map.SchemaKey} is not generated and exist multiple times - location-calculation is missing.");
                }

                var generatorModel = new SyntaxGeneratorContractModel(
                    projectOptions,
                    map.SchemaKey,
                    projectOptions.Document.Components.Schemas.First(x => x.Key.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)).Value,
                    map.SegmentName);

                generatorModel.GenerateCode();
                sgContractModels.Add(generatorModel);
            }
        }
    }
}
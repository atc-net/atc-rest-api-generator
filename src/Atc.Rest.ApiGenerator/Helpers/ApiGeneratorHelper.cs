// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Helpers;

public static class ApiGeneratorHelper
{
    public static void CollectMissingContractModelFromOperationSchemaMappings(
        ILogger logger,
        ApiProjectOptions projectOptions,
        IList<ApiOperationSchemaMap> operationSchemaMappings,
        List<SyntaxGeneratorContractModel> sgContractModels)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectOptions);
        ArgumentNullException.ThrowIfNull(operationSchemaMappings);
        ArgumentNullException.ThrowIfNull(sgContractModels);

        var missingOperationSchemaMappings = new List<ApiOperationSchemaMap>();
        foreach (var map in operationSchemaMappings)
        {
            if (sgContractModels.FirstOrDefault(x => x.ApiSchemaKey.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)) is null)
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
                logger,
                projectOptions,
                map.SchemaKey,
                projectOptions.Document.Components.Schemas.First(x => x.Key.Equals(map.SchemaKey, StringComparison.OrdinalIgnoreCase)).Value,
                map.SegmentName);

            generatorModel.GenerateCode();
            sgContractModels.Add(generatorModel);
        }
    }
}
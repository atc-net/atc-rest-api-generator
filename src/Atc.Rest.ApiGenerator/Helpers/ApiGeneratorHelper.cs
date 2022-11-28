// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Rest.ApiGenerator.Helpers;

public static class ApiGeneratorHelper
{
    public static void CollectMissingContractModelFromOperationSchemaMappings(
        ILogger logger,
        ApiProjectOptions projectOptions,
        IList<ApiOperation> apiOperations,
        List<SyntaxGeneratorContractModel> sgContractModels)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectOptions);
        ArgumentNullException.ThrowIfNull(apiOperations);
        ArgumentNullException.ThrowIfNull(sgContractModels);

        var missingApiOperations = new List<ApiOperation>();
        foreach (var apiOperation in apiOperations)
        {
            if (sgContractModels.FirstOrDefault(x => x.ApiSchemaKey.Equals(apiOperation.Model.Name, StringComparison.OrdinalIgnoreCase)) is null)
            {
                missingApiOperations.Add(apiOperation);
            }
        }

        foreach (var apiOperation in missingApiOperations)
        {
            if (missingApiOperations.Count(x => x.Model.Name.Equals(apiOperation.Model.Name, StringComparison.OrdinalIgnoreCase)) > 1)
            {
                throw new NotImplementedException($"SchemaKey: {apiOperation.Model.Name} is not generated and exist multiple times - location-calculation is missing.");
            }

            var generatorModel = new SyntaxGeneratorContractModel(
                logger,
                projectOptions,
                apiOperation.Model.Name,
                projectOptions.Document.Components.Schemas.First(x => x.Key.Equals(apiOperation.Model.Name, StringComparison.OrdinalIgnoreCase)).Value,
                apiOperation.SegmentName);

            generatorModel.GenerateCode();
            sgContractModels.Add(generatorModel);
        }
    }
}
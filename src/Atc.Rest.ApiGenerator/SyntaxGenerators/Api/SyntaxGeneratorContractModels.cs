// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractModels : ISyntaxGeneratorContractModels
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractModels(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        IList<ApiOperation> operationSchemaMappings,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<ApiOperation> OperationSchemaMappings { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorContractModel> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorContractModel>();

        var apiOperations = OperationSchemaMappings
            .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                        x.SegmentName.Equals(FocusOnSegmentName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var apiOperationModels = GetDistinctApiOperationModels(apiOperations);

        foreach (var apiOperationModel in apiOperationModels)
        {
            var apiSchema = ApiProjectOptions.Document.Components.Schemas.First(x => x.Key.Equals(apiOperationModel.Name, StringComparison.OrdinalIgnoreCase));
            if (apiOperationModel.IsEnum)
            {
                var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(logger, ApiProjectOptions, apiSchema.Key, apiSchema.Value, FocusOnSegmentName);
                syntaxGeneratorContractModel.GenerateCode();
                list.Add(syntaxGeneratorContractModel);
            }
            else
            {
                if (apiOperationModel.IsShared)
                {
                    var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(logger, ApiProjectOptions, apiSchema.Key, apiSchema.Value, "#");
                    syntaxGeneratorContractModel.GenerateCode();
                    list.Add(syntaxGeneratorContractModel);
                }
                else
                {
                    var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(logger, ApiProjectOptions, apiSchema.Key, apiSchema.Value, FocusOnSegmentName);
                    syntaxGeneratorContractModel.GenerateCode();
                    list.Add(syntaxGeneratorContractModel);
                }
            }
        }

        return list;
    }

    private static List<ApiOperationModel> GetDistinctApiOperationModels(
        List<ApiOperation> apiOperations)
    {
        var result = new List<ApiOperationModel>();

        foreach (var apiOperation in apiOperations)
        {
            var apiOperationModel = result.FirstOrDefault(x => x.Name.Equals(apiOperation.Model.Name, StringComparison.Ordinal));
            if (apiOperationModel is null)
            {
                result.Add(apiOperation.Model);
            }
        }

        return result;
    }
}
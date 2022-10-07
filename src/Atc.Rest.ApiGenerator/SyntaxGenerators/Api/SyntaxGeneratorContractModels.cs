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
        IList<ApiOperationSchemaMap> operationSchemaMappings,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<ApiOperationSchemaMap> OperationSchemaMappings { get; }

    public string FocusOnSegmentName { get; }

    public List<SyntaxGeneratorContractModel> GenerateSyntaxTrees()
    {
        var list = new List<SyntaxGeneratorContractModel>();

        var apiOperationSchemaMaps = OperationSchemaMappings
            .Where(x => x.LocatedArea is ApiSchemaMapLocatedAreaType.Response or ApiSchemaMapLocatedAreaType.RequestBody &&
                        x.SegmentName.Equals(FocusOnSegmentName, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var schemaKeys = apiOperationSchemaMaps
            .Select(x => x.SchemaKey)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(x => x)
            .ToList();

        foreach (var schemaKey in schemaKeys)
        {
            var apiSchema = ApiProjectOptions.Document.Components.Schemas.First(x => x.Key.Equals(schemaKey, StringComparison.OrdinalIgnoreCase));
            if (apiSchema.Value.IsSchemaEnumOrPropertyEnum())
            {
                var syntaxGeneratorContractModel = new SyntaxGeneratorContractModel(logger, ApiProjectOptions, apiSchema.Key, apiSchema.Value, FocusOnSegmentName);
                syntaxGeneratorContractModel.GenerateCode();
                list.Add(syntaxGeneratorContractModel);
            }
            else
            {
                var isShared = OperationSchemaMappings.IsShared(schemaKey);
                if (isShared)
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
}
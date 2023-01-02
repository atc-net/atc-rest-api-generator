// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractModel
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractModel(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        string apiSchemaKey,
        OpenApiSchema apiSchema,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        ApiSchemaKey = apiSchemaKey ?? throw new ArgumentNullException(nameof(apiSchemaKey));
        ApiSchema = apiSchema ?? throw new ArgumentNullException(nameof(apiSchema));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        if (FocusOnSegmentName == "#")
        {
            IsSharedContract = true;
        }

        IsForClient = false;
        UseOwnFolder = true;
    }

    private ApiProjectOptions ApiProjectOptions { get; }

    private bool IsSharedContract { get; }

    public string ApiSchemaKey { get; }

    public OpenApiSchema ApiSchema { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool IsEnum { get; private set; }

    public bool IsForClient { get; set; }

    public bool UseOwnFolder { get; set; }

    public bool GenerateCode()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();
        Code = compilationUnit;
        return true;
    }

    public override string ToString()
        => $"{nameof(ApiSchemaKey)}: {ApiSchemaKey}, SegmentName: {FocusOnSegmentName}, IsShared: {IsSharedContract}, {nameof(IsEnum)}: {IsEnum}";
}
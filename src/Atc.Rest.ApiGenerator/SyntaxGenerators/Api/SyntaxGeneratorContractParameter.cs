// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractParameter
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractParameter(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        ApiOperationType = apiOperationType;
        ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));

        IsForClient = false;
        UseOwnFolder = true;
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<OpenApiParameter> GlobalPathParameters { get; }

    public OperationType ApiOperationType { get; }

    public OpenApiOperation ApiOperation { get; }

    public string FocusOnSegmentName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool IsForClient { get; set; }

    public bool UseOwnFolder { get; set; }

    public bool GenerateCode()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();
        Code = compilationUnit;
        return true;
    }

    public string ToCodeAsString()
    {
        if (Code is null)
        {
            GenerateCode();
        }

        if (Code is null)
        {
            return $"Syntax generate problem for contract-parameter for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .FormatAutoPropertiesOnOneLine()
            .FormatPublicPrivateLines()
            .FormatDoubleLines()
            .EnsureFileScopedNamespace();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var parameterName = ApiOperation.GetOperationName() + NameConstants.ContractParameters;

        var file = IsForClient
            ? Helpers.DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ClientRequestParameters, parameterName)
            : UseOwnFolder
                ? Helpers.DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractParameters, parameterName)
                : Helpers.DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, parameterName);

        ToFile(new FileInfo(file));
    }

    public void ToFile(
        FileInfo file)
    {
        ArgumentNullException.ThrowIfNull(file);

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            ApiProjectOptions.PathForSrcGenerate,
            file,
            ContentWriterArea.Src,
            ToCodeAsString());
    }

    public override string ToString()
        => $"OperationType: {ApiOperationType}, OperationName: {ApiOperation.GetOperationName()}, SegmentName: {FocusOnSegmentName}";
}
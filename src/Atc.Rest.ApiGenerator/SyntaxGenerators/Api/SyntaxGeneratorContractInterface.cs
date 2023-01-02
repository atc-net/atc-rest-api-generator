namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorContractInterface
{
    private readonly ILogger logger;

    public SyntaxGeneratorContractInterface(
        ILogger logger,
        ApiProjectOptions apiProjectOptions,
        IList<OpenApiParameter> globalPathParameters,
        OperationType apiOperationType,
        OpenApiOperation apiOperation,
        string focusOnSegmentName,
        bool hasParametersOrRequestBody)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        GlobalPathParameters = globalPathParameters ?? throw new ArgumentNullException(nameof(globalPathParameters));
        ApiOperationType = apiOperationType;
        ApiOperation = apiOperation ?? throw new ArgumentNullException(nameof(apiOperation));
        FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        HasParametersOrRequestBody = hasParametersOrRequestBody;
    }

    public ApiProjectOptions ApiProjectOptions { get; }

    public IList<OpenApiParameter> GlobalPathParameters { get; }

    public OperationType ApiOperationType { get; }

    public OpenApiOperation ApiOperation { get; }

    public string FocusOnSegmentName { get; }

    public bool HasParametersOrRequestBody { get; }

    public CompilationUnitSyntax? Code { get; private set; }

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
            return $"Syntax generate problem for contract-interface for apiOperation: {ApiOperation}";
        }

        return Code
            .NormalizeWhitespace()
            .ToFullString()
            .EnsureEnvironmentNewLines()
            .EnsureFileScopedNamespace();
    }

    public void ToFile()
    {
        var area = FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var interfaceName = "I" + ApiOperation.GetOperationName() + NameConstants.ContractHandler;
        var file = Helpers.DirectoryInfoHelper.GetCsFileNameForContract(ApiProjectOptions.PathForContracts, area, NameConstants.ContractInterfaces, interfaceName);
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
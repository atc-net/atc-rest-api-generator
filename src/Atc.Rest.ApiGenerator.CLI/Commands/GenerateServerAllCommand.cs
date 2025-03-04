namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateServerAllCommand : AsyncCommand<ServerAllCommandSettings>
{
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger<GenerateServerAllCommand> logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly IAtcCodingRulesUpdater atcCodingRulesUpdater;
    private readonly IOpenApiDocumentValidator openApiDocumentValidator;

    public GenerateServerAllCommand(
        ILoggerFactory loggerFactory,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        IAtcCodingRulesUpdater atcCodingRulesUpdater,
        IOpenApiDocumentValidator openApiDocumentValidator)
    {
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<GenerateServerAllCommand>();
        this.apiOperationExtractor = apiOperationExtractor;
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.atcCodingRulesUpdater = atcCodingRulesUpdater;
        this.openApiDocumentValidator = openApiDocumentValidator;
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ServerAllCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
    private async Task<int> ExecuteInternalAsync(
        ServerAllCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

        var projectPrefixName = settings.ProjectPrefixName;
        var outputSlnPath = settings.OutputSlnPath;
        var outputSrcPath = new DirectoryInfo(settings.OutputSrcPath);

        DirectoryInfo? outputTestPath = null;

        if (settings.OutputTestPath is not null &&
            settings.OutputTestPath.IsSet)
        {
            outputTestPath = new DirectoryInfo(settings.OutputTestPath.Value);
        }

        var apiOptions = await ApiOptionsHelper.CreateDefault(settings);
        var apiSpecificationContentReader = new ApiSpecificationContentReader();
        var apiDocumentContainer = apiSpecificationContentReader.CombineAndGetApiDocumentContainer(logger, settings.SpecificationPath);
        if (apiDocumentContainer.Exception is not null)
        {
            logger.LogError(apiDocumentContainer.Exception, $"{EmojisConstants.Error} Reading specification failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        var shouldScaffoldCodingRules = CodingRulesHelper.ShouldScaffoldCodingRules(outputSlnPath, settings.DisableCodingRules);
        var isUsingCodingRules = CodingRulesHelper.IsUsingCodingRules(outputSlnPath, settings.DisableCodingRules);

        if (shouldScaffoldCodingRules &&
            !NetworkInformationHelper.HasHttpConnection())
        {
            System.Console.WriteLine("This tool requires internet connection!");
            return ConsoleExitStatusCodes.Failure;
        }

        try
        {
            if (!openApiDocumentValidator.IsValid(
                    apiOptions.Validation,
                    apiOptions.IncludeDeprecatedOperations,
                    apiDocumentContainer))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerApi(
                loggerFactory,
                apiOperationExtractor,
                nugetPackageReferenceProvider,
                projectPrefixName,
                outputSrcPath,
                outputTestPath,
                apiDocumentContainer,
                apiOptions,
                isUsingCodingRules))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerDomain(
                    loggerFactory,
                    apiOperationExtractor,
                    nugetPackageReferenceProvider,
                    projectPrefixName,
                    outputSrcPath,
                    outputTestPath,
                    apiDocumentContainer,
                    apiOptions,
                    isUsingCodingRules,
                    outputSrcPath))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerHost(
                    loggerFactory,
                    apiOperationExtractor,
                    nugetPackageReferenceProvider,
                    projectPrefixName,
                    outputSrcPath,
                    outputTestPath,
                    apiDocumentContainer,
                    apiOptions,
                    isUsingCodingRules,
                    outputSrcPath,
                    outputSrcPath))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (!GenerateHelper.GenerateServerSln(
                    loggerFactory,
                    projectPrefixName,
                    outputSlnPath,
                    outputSrcPath,
                    outputTestPath,
                    apiOptions.Generator.AspNetOutputType))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (shouldScaffoldCodingRules &&
                !atcCodingRulesUpdater.Scaffold(
                    outputSlnPath,
                    outputSrcPath,
                    apiOptions.Generator.AspNetOutputType == AspNetOutputType.Mvc ? outputTestPath : null))
            {
                return ConsoleExitStatusCodes.Failure;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"{EmojisConstants.Error} Generation failed.");
            return ConsoleExitStatusCodes.Failure;
        }

        logger.LogInformation($"{EmojisConstants.Success} Done");
        return ConsoleExitStatusCodes.Success;
    }
}
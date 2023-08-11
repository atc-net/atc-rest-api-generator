namespace Atc.Rest.ApiGenerator.CLI.Commands;

public class GenerateServerApiCommand : AsyncCommand<ServerApiCommandSettings>
{
    private readonly ILogger<GenerateServerApiCommand> logger;
    private readonly IApiOperationExtractor apiOperationExtractor;
    private readonly INugetPackageReferenceProvider nugetPackageReferenceProvider;
    private readonly IOpenApiDocumentValidator openApiDocumentValidator;

    public GenerateServerApiCommand(
        ILogger<GenerateServerApiCommand> logger,
        IApiOperationExtractor apiOperationExtractor,
        INugetPackageReferenceProvider nugetPackageReferenceProvider,
        IOpenApiDocumentValidator openApiDocumentValidator)
    {
        this.logger = logger;
        this.apiOperationExtractor = apiOperationExtractor;
        this.nugetPackageReferenceProvider = nugetPackageReferenceProvider;
        this.openApiDocumentValidator = openApiDocumentValidator;
    }

    public override Task<int> ExecuteAsync(
        CommandContext context,
        ServerApiCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(settings);
        return ExecuteInternalAsync(settings);
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "OK.")]
    [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "OK.")]
    private async Task<int> ExecuteInternalAsync(
        ServerApiCommandSettings settings)
    {
        ConsoleHelper.WriteHeader();

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

        var shouldScaffoldCodingRules = CodingRulesHelper.ShouldScaffoldCodingRules(settings.OutputPath, settings.DisableCodingRules);
        var isUsingCodingRules = CodingRulesHelper.IsUsingCodingRules(settings.OutputPath, settings.DisableCodingRules);

        var aspNetOutputType = AspNetOutputType.Mvc;

        if (settings.AspNetOutputType.IsSet)
        {
            aspNetOutputType = settings.AspNetOutputType.Value;
        }

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
                    apiDocumentContainer))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            if (aspNetOutputType == AspNetOutputType.MinimalApi)
            {
                // TODO: Implement
                return ConsoleExitStatusCodes.Success;
            }
            else
            {
                if (!GenerateHelper.GenerateServerApi(
                        logger,
                        apiOperationExtractor,
                        nugetPackageReferenceProvider,
                        settings.ProjectPrefixName,
                        new DirectoryInfo(settings.OutputPath),
                        outputTestPath,
                        apiDocumentContainer,
                        apiOptions,
                        isUsingCodingRules,
                        settings.RemoveNamespaceGroupSeparatorInGlobalUsings))
                {
                    return ConsoleExitStatusCodes.Failure;
                }
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
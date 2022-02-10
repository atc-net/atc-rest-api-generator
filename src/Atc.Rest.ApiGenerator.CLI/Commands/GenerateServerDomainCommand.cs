namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    public class GenerateServerDomainCommand : AsyncCommand<ServerDomainCommandSettings>
    {
        private readonly ILogger<GenerateServerDomainCommand> logger;

        public GenerateServerDomainCommand(ILogger<GenerateServerDomainCommand> logger) => this.logger = logger;

        public override Task<int> ExecuteAsync(
            CommandContext context,
            ServerDomainCommandSettings settings)
        {
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(settings);
            return ExecuteInternalAsync(settings);
        }

        private async Task<int> ExecuteInternalAsync(
            ServerDomainCommandSettings settings)
        {
            ConsoleHelper.WriteHeader();

            DirectoryInfo? outputTestPath = null;

            if (settings.OutputTestPath is not null &&
                settings.OutputTestPath.IsSet)
            {
                outputTestPath = new DirectoryInfo(settings.OutputTestPath.Value);
            }

            var apiOptions = await ApiOptionsHelper.CreateApiOptions(settings);
            var apiDocument = OpenApiDocumentHelper.CombineAndGetApiDocument(settings.SpecificationPath);

            var logItems = new List<LogKeyValueItem>();

            try
            {
                logItems.AddRange(OpenApiDocumentHelper.Validate(apiDocument, apiOptions.Validation));

                if (logItems.HasAnyErrorsLogIfNeeded(logger))
                {
                    return ConsoleExitStatusCodes.Failure;
                }

                logItems.AddRange(GenerateHelper.GenerateServerDomain(
                    settings.ProjectPrefixName,
                    new DirectoryInfo(settings.OutputPath),
                    outputTestPath,
                    apiDocument,
                    apiOptions,
                    new DirectoryInfo(settings.ApiPath)));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Generation failed.");
                return ConsoleExitStatusCodes.Failure;
            }

            if (logItems.HasAnyErrorsLogIfNeeded(logger))
            {
                return ConsoleExitStatusCodes.Failure;
            }

            logger.LogInformation("Done");
            return ConsoleExitStatusCodes.Success;
        }
    }
}
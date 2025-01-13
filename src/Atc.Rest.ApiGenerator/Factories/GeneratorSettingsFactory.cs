namespace Atc.Rest.ApiGenerator.Factories;

public static class GeneratorSettingsFactory
{
    public static GeneratorSettings Create(
        Version version,
        string projectName,
        DirectoryInfo projectPath,
        ApiOptionsGenerator apiOptionsGenerator,
        bool includeDeprecatedOperations)
    {
        ArgumentNullException.ThrowIfNull(apiOptionsGenerator);

        var generatorSettings = new GeneratorSettings(
            version,
            projectName,
            projectPath)
        {
            UseProblemDetailsAsDefaultResponseBody = apiOptionsGenerator.Response.UseProblemDetailsAsDefaultBody,
            UsePartialClassForContracts = apiOptionsGenerator.UsePartialClassForContracts,
            UsePartialClassForEndpoints = apiOptionsGenerator.UsePartialClassForEndpoints,
            IncludeDeprecatedOperations = includeDeprecatedOperations,
        };

        if (!string.IsNullOrEmpty(apiOptionsGenerator.EndpointsLocation))
        {
            generatorSettings.EndpointsLocation = apiOptionsGenerator.EndpointsLocation;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.EndpointsNamespace))
        {
            generatorSettings.EndpointsNamespace = apiOptionsGenerator.EndpointsNamespace;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.ContractsLocation))
        {
            generatorSettings.ContractsLocation = apiOptionsGenerator.ContractsLocation;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.ContractsNamespace))
        {
            generatorSettings.ContractsNamespace = apiOptionsGenerator.ContractsNamespace;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.HandlersLocation))
        {
            generatorSettings.HandlersLocation = apiOptionsGenerator.HandlersLocation;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.HandlersNamespace))
        {
            generatorSettings.HandlersNamespace = apiOptionsGenerator.HandlersNamespace;
        }

        return generatorSettings;
    }
}
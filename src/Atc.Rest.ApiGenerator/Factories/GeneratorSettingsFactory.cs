namespace Atc.Rest.ApiGenerator.Factories;

public static class GeneratorSettingsFactory
{
    public static GeneratorSettings Create(
        Version version,
        string projectName,
        DirectoryInfo projectPath,
        ApiOptionsGenerator apiOptionsGenerator)
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
            IncludeDeprecatedOperations = apiOptionsGenerator.IncludeDeprecated,
        };

        if (!string.IsNullOrEmpty(apiOptionsGenerator.EndpointsLocation))
        {
            generatorSettings.EndpointsLocation = apiOptionsGenerator.EndpointsLocation;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.ContractsLocation))
        {
            generatorSettings.ContractsLocation = apiOptionsGenerator.ContractsLocation;
        }

        if (!string.IsNullOrEmpty(apiOptionsGenerator.HandlersLocation))
        {
            generatorSettings.HandlersLocation = apiOptionsGenerator.HandlersLocation;
        }

        return generatorSettings;
    }
}
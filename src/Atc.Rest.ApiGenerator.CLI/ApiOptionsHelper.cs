// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.CLI;

[SuppressMessage("Info Code Smell", "S4457:Split this method into two", Justification = "OK for now.")]
public static class ApiOptionsHelper
{
    public static async Task<ApiOptions> CreateDefault(
        BaseConfigurationCommandSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        var optionsPath = settings.GetOptionsPath();
        if (string.IsNullOrEmpty(optionsPath))
        {
            var fileInfo = new FileInfo(settings.SpecificationPath);
            if (!string.IsNullOrEmpty(fileInfo.DirectoryName))
            {
                optionsPath = fileInfo.DirectoryName;
            }
        }

        if (string.IsNullOrEmpty(optionsPath))
        {
            return new ApiOptions();
        }

        var options = await CreateDefault(optionsPath);

        ApplyValidationOverrides(options, settings);
        ApplyGeneratorOverrides(options, settings);

        return options;
    }

    public static async Task<ApiOptions> CreateDefault(
        string optionsPath)
    {
        ArgumentNullException.ThrowIfNull(optionsPath);

        var fileInfo = GetOptionsFile(optionsPath);
        if (!fileInfo.Exists)
        {
            return new ApiOptions();
        }

        var options = await FileHelper<ApiOptions>.ReadJsonFileAndDeserializeAsync(fileInfo);
        return options ?? new ApiOptions();
    }

    public static async Task<(bool, string)> CreateOptionsFile(
        string optionsPath)
    {
        ArgumentNullException.ThrowIfNull(optionsPath);

        var options = new ApiOptions();

        var fileInfo = GetOptionsFile(optionsPath);
        if (fileInfo.Exists)
        {
            options = await FileHelper<ApiOptions>.ReadJsonFileAndDeserializeAsync(fileInfo) ?? new ApiOptions();
        }

        await FileHelper<ApiOptions>.WriteModelToJsonFileAsync(fileInfo, options);
        return (true, string.Empty);
    }

    public static async Task<(bool, string)> ValidateOptionsFile(
        string optionsPath)
    {
        ArgumentNullException.ThrowIfNull(optionsPath);

        var fileInfo = GetOptionsFile(optionsPath);
        if (!fileInfo.Exists)
        {
            return (false, "File does not exist");
        }

        try
        {
            var options = await FileHelper<ApiOptions>.ReadJsonFileAndDeserializeAsync(fileInfo);
            return options is null
                ? (false, "File is invalid")
                : (true, string.Empty);
        }
        catch (Exception ex)
        {
            return (false, $"File is invalid: {ex.Message}");
        }
    }

    private static FileInfo GetOptionsFile(
        string optionsPath)
    {
        return optionsPath.EndsWith(".json", StringComparison.Ordinal)
            ? new FileInfo(optionsPath)
            : new FileInfo(Path.Combine(optionsPath, "ApiGeneratorOptions.json"));
    }

    private static void ApplyValidationOverrides(
        ApiOptions apiOptions,
        BaseConfigurationCommandSettings settings)
    {
        if (settings.StrictMode)
        {
            apiOptions.Validation.StrictMode = settings.StrictMode;
        }

        if (settings.OperationIdValidation)
        {
            apiOptions.Validation.OperationIdValidation = settings.OperationIdValidation;
        }

        if (settings.OperationIdCasingStyle.IsSet)
        {
            apiOptions.Validation.OperationIdCasingStyle = settings.OperationIdCasingStyle.Value;
        }

        if (settings.ModelNameCasingStyle.IsSet)
        {
            apiOptions.Validation.ModelNameCasingStyle = settings.ModelNameCasingStyle.Value;
        }

        if (settings.ModelPropertyNameCasingStyle.IsSet)
        {
            apiOptions.Validation.ModelPropertyNameCasingStyle = settings.ModelPropertyNameCasingStyle.Value;
        }
    }

    private static void ApplyGeneratorOverrides(
        ApiOptions apiOptions,
        BaseConfigurationCommandSettings settings)
    {
        if (settings is BaseServerCommandSettings serverCommandSettings)
        {
            if (serverCommandSettings.AspNetOutputType.IsSet &&
                serverCommandSettings.AspNetOutputType.Value != apiOptions.Generator.AspNetOutputType)
            {
                apiOptions.Generator.AspNetOutputType = serverCommandSettings.AspNetOutputType.Value;
            }

            if (serverCommandSettings.SwaggerThemeMode.IsSet &&
                serverCommandSettings.SwaggerThemeMode.Value != apiOptions.Generator.SwaggerThemeMode)
            {
                apiOptions.Generator.SwaggerThemeMode = serverCommandSettings.SwaggerThemeMode.Value;
            }

            if (serverCommandSettings.UseProblemDetailsAsDefaultResponseBody)
            {
                apiOptions.Generator.Response.UseProblemDetailsAsDefaultBody = serverCommandSettings.UseProblemDetailsAsDefaultResponseBody;
            }

            if (serverCommandSettings.ProjectPrefixName is not null)
            {
                apiOptions.Generator.ProjectName = serverCommandSettings.ProjectPrefixName;
            }

            if (serverCommandSettings.EndpointsLocation is not null &&
                serverCommandSettings.EndpointsLocation.IsSet)
            {
                apiOptions.Generator.EndpointsLocation = serverCommandSettings.EndpointsLocation.Value;
            }

            if (serverCommandSettings.EndpointsNamespace is not null &&
                serverCommandSettings.EndpointsNamespace.IsSet)
            {
                apiOptions.Generator.EndpointsNamespace = serverCommandSettings.EndpointsNamespace.Value;
            }

            if (serverCommandSettings.ContractsLocation is not null &&
                serverCommandSettings.ContractsLocation.IsSet)
            {
                apiOptions.Generator.ContractsLocation = serverCommandSettings.ContractsLocation.Value;
            }

            if (serverCommandSettings.ContractsNamespace is not null &&
                serverCommandSettings.ContractsNamespace.IsSet)
            {
                apiOptions.Generator.ContractsNamespace = serverCommandSettings.ContractsNamespace.Value;
            }

            if (serverCommandSettings.HandlersLocation is not null &&
                serverCommandSettings.HandlersLocation.IsSet)
            {
                apiOptions.Generator.HandlersLocation = serverCommandSettings.HandlersLocation.Value;
            }

            if (serverCommandSettings.HandlersNamespace is not null &&
                serverCommandSettings.HandlersNamespace.IsSet)
            {
                apiOptions.Generator.HandlersNamespace = serverCommandSettings.HandlersNamespace.Value;
            }

            if (serverCommandSettings.UsePartialClassForContracts)
            {
                apiOptions.Generator.UsePartialClassForContracts = serverCommandSettings.UsePartialClassForContracts;
            }

            if (serverCommandSettings.UsePartialClassForEndpoints)
            {
                apiOptions.Generator.UsePartialClassForEndpoints = serverCommandSettings.UsePartialClassForEndpoints;
            }

            if (serverCommandSettings.RemoveNamespaceGroupSeparatorInGlobalUsings)
            {
                apiOptions.Generator.RemoveNamespaceGroupSeparatorInGlobalUsings = serverCommandSettings.RemoveNamespaceGroupSeparatorInGlobalUsings;
            }
        }

        switch (settings)
        {
            case ClientApiCommandSettings clientApiCommandSettings:
            {
                if (clientApiCommandSettings.UseProblemDetailsAsDefaultResponseBody)
                {
                    apiOptions.Generator.Response.UseProblemDetailsAsDefaultBody = clientApiCommandSettings.UseProblemDetailsAsDefaultResponseBody;
                }

                if (clientApiCommandSettings.ProjectPrefixName is not null)
                {
                    apiOptions.Generator.ProjectName = clientApiCommandSettings.ProjectPrefixName;
                }

                if (clientApiCommandSettings.RemoveNamespaceGroupSeparatorInGlobalUsings)
                {
                    apiOptions.Generator.RemoveNamespaceGroupSeparatorInGlobalUsings = clientApiCommandSettings.RemoveNamespaceGroupSeparatorInGlobalUsings;
                }

                if (string.IsNullOrEmpty(apiOptions.Generator.ProjectSuffixName))
                {
                    apiOptions.Generator.ProjectSuffixName = $"{ContentGeneratorConstants.DefaultHttpClientName}.Generated";
                }

                if (clientApiCommandSettings.EndpointsLocation is not null &&
                    clientApiCommandSettings.EndpointsLocation.IsSet)
                {
                    apiOptions.Generator.EndpointsLocation = clientApiCommandSettings.EndpointsLocation.Value;
                }

                if (clientApiCommandSettings.EndpointsNamespace is not null &&
                    clientApiCommandSettings.EndpointsNamespace.IsSet)
                {
                    apiOptions.Generator.EndpointsNamespace = clientApiCommandSettings.EndpointsNamespace.Value;
                }

                if (clientApiCommandSettings.ContractsLocation is not null &&
                    clientApiCommandSettings.ContractsLocation.IsSet)
                {
                    apiOptions.Generator.ContractsLocation = clientApiCommandSettings.ContractsLocation.Value;
                }

                if (clientApiCommandSettings.ContractsNamespace is not null &&
                    clientApiCommandSettings.ContractsNamespace.IsSet)
                {
                    apiOptions.Generator.ContractsNamespace = clientApiCommandSettings.ContractsNamespace.Value;
                }

                if (clientApiCommandSettings.UsePartialClassForContracts)
                {
                    apiOptions.Generator.UsePartialClassForContracts = clientApiCommandSettings.UsePartialClassForContracts;
                }

                if (clientApiCommandSettings.UsePartialClassForEndpoints)
                {
                    apiOptions.Generator.UsePartialClassForEndpoints = clientApiCommandSettings.UsePartialClassForEndpoints;
                }

                apiOptions.Generator.Client ??= new ApiOptionsGeneratorClient();

                if (clientApiCommandSettings.HttpClientName is not null &&
                    clientApiCommandSettings.HttpClientName.IsSet)
                {
                    apiOptions.Generator.Client.HttpClientName = clientApiCommandSettings.HttpClientName.Value;
                }
                else if (ContentGeneratorConstants.DefaultHttpClientName.Equals(apiOptions.Generator.Client.HttpClientName, StringComparison.Ordinal))
                {
                    var baseGenerateCommandSettings = (BaseGenerateCommandSettings)settings;
                    apiOptions.Generator.Client.HttpClientName = $"{baseGenerateCommandSettings.ProjectPrefixName}-{ContentGeneratorConstants.DefaultHttpClientName}";
                }

                if (clientApiCommandSettings.ExcludeEndpointGeneration)
                {
                    apiOptions.Generator.Client.ExcludeEndpointGeneration = clientApiCommandSettings.ExcludeEndpointGeneration;
                }

                break;
            }
        }

        apiOptions.Generator.ProjectName = apiOptions.Generator.ProjectName.EnsureNamespaceFormatPart();
        apiOptions.Generator.ProjectSuffixName = apiOptions.Generator.ProjectSuffixName.EnsureNamespaceFormatPart();
    }
}
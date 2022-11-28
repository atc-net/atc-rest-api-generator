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

        var fileInfo = GetOptionsFile(optionsPath);
        if (fileInfo.Exists)
        {
            return (false, "File already exist");
        }

        var options = new ApiOptions();

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

        var options = await FileHelper<ApiOptions>.ReadJsonFileAndDeserializeAsync(fileInfo);
        return options is null
            ? (false, "File is invalid")
            : (true, string.Empty);
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
}
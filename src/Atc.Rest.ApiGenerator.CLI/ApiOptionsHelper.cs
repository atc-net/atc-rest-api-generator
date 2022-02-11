namespace Atc.Rest.ApiGenerator.CLI;

public static class ApiOptionsHelper
{
    public static async Task<ApiOptions> CreateApiOptions(
        BaseConfigurationCommandSettings settings)
    {
        if (settings.OptionsPath is null ||
            !settings.OptionsPath.IsSet)
        {
            return new ApiOptions();
        }

        var fileInfo = new FileInfo(settings.OptionsPath.Value);
        if (!fileInfo.Exists)
        {
            throw new FileNotFoundException("Could not find options file.", settings.OptionsPath.Value);
        }

        var options = await DeserializeFile(fileInfo);
        if (options is null)
        {
            throw new SerializationException($"Could not read options file '{settings.OptionsPath.Value}'.");
        }

        ApplyValidationOverrides(options, settings);
        ApplyGeneratorOverrides(options, settings);

        return options;
    }

    private static void ApplyValidationOverrides(
        ApiOptions apiOptions,
        BaseConfigurationCommandSettings settings)
    {
        if (settings.StrictMode)
        {
            apiOptions.Validation.StrictMode = settings.StrictMode;
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
        if (settings.UseAuthorization)
        {
            apiOptions.Generator.UseAuthorization = settings.UseAuthorization;
        }
    }

    private static async Task<ApiOptions?> DeserializeFile(
        FileInfo fileInfo)
    {
        var serializeOptions = JsonSerializerOptionsFactory.Create();
        using var stream = new StreamReader(fileInfo.FullName);
        var json = await stream.ReadToEndAsync();
        return JsonSerializer.Deserialize<ApiOptions>(json, serializeOptions);
    }
}
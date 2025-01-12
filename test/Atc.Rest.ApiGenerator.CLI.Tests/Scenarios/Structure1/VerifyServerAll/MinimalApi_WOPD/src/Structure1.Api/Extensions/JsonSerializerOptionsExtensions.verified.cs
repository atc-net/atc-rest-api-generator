namespace Structure1.Api.Extensions;

public static class JsonSerializerOptionsExtensions
{
    public static Microsoft.AspNetCore.Http.Json.JsonOptions Configure(
        this System.Text.Json.JsonSerializerOptions jsonSerializerOptions,
        Microsoft.AspNetCore.Http.Json.JsonOptions options)
    {
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        ArgumentNullException.ThrowIfNull(options);

        options.SerializerOptions.DefaultIgnoreCondition = jsonSerializerOptions.DefaultIgnoreCondition;
        options.SerializerOptions.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;
        options.SerializerOptions.WriteIndented = jsonSerializerOptions.WriteIndented;
        options.SerializerOptions.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;

        foreach (var jsonConverter in jsonSerializerOptions.Converters)
        {
            options.SerializerOptions.Converters.Add(jsonConverter);
        }

        return options;
    }

    public static Microsoft.AspNetCore.Mvc.JsonOptions Configure(
        this System.Text.Json.JsonSerializerOptions jsonSerializerOptions,
        Microsoft.AspNetCore.Mvc.JsonOptions options)
    {
        ArgumentNullException.ThrowIfNull(jsonSerializerOptions);
        ArgumentNullException.ThrowIfNull(options);

        options.JsonSerializerOptions.DefaultIgnoreCondition = jsonSerializerOptions.DefaultIgnoreCondition;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = jsonSerializerOptions.PropertyNameCaseInsensitive;
        options.JsonSerializerOptions.WriteIndented = jsonSerializerOptions.WriteIndented;
        options.JsonSerializerOptions.PropertyNamingPolicy = jsonSerializerOptions.PropertyNamingPolicy;

        foreach (var jsonConverter in jsonSerializerOptions.Converters)
        {
            options.JsonSerializerOptions.Converters.Add(jsonConverter);
        }

        return options;
    }
}
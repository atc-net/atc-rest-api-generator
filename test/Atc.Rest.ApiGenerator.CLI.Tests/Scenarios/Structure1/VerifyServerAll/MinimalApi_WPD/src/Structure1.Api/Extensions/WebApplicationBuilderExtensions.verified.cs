namespace Structure1.Api.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureLogging(
        this WebApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Information);
            logging.AddConsole();
            logging.AddDebug();
        });

        builder.Services.AddHttpLogging(logging =>
        {
            logging.LoggingFields = HttpLoggingFields.All;
            logging.RequestHeaders.Add("Authorization");
            logging.RequestBodyLogLimit = 4096;
            logging.ResponseBodyLogLimit = 4096;
            logging.MediaTypeOptions.AddText("application/json");
        });

        builder.Services.AddApplicationInsightsTelemetry();

        builder.Logging.AddFilter<ApplicationInsightsLoggerProvider>(
            "Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware",
            LogLevel.Trace);
    }
}
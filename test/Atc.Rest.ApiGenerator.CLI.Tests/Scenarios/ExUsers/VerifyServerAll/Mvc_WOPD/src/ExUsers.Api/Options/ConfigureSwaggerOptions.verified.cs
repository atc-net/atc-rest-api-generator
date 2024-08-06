namespace ExUsers.Api.Options;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;
    private readonly IWebHostEnvironment environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    /// <param name="environment">The environment.</param>
    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider,
        IWebHostEnvironment environment)
    {
        this.provider = provider;
        this.environment = environment;
    }

    /// <inheritdoc />
    public void Configure(
        SwaggerGenOptions options)
    {
        // Add a swagger document for each discovered API version
        // note: you might choose to skip or document deprecated API versions differently
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private OpenApiInfo CreateInfoForApiVersion(
        ApiVersionDescription description)
    {
        var text = new StringBuilder("Demo Users Api - SingleFileVersion");
        var info = new OpenApiInfo
        {
            Title = $"{environment.ApplicationName} {description.GroupName.ToUpperInvariant()}",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact
            {
                Name = "atc-net A/S",
            },
        };

        if (description.IsDeprecated)
        {
            text.Append(" This API version has been deprecated.");
        }

        if (description.SunsetPolicy is { } policy)
        {
            if (policy.Date is { } when)
            {
                text.Append(" The API will be sunset on ")
                    .Append(when.Date.ToShortDateString())
                    .Append('.');
            }

            if (policy.HasLinks)
            {
                text.AppendLine();

                foreach (var link in policy.Links)
                {
                    if (link.Type != "text/html")
                    {
                        continue;
                    }

                    text.AppendLine();

                    if (link.Title.HasValue)
                    {
                        text.Append(link.Title.Value).Append(": ");
                    }

                    text.Append(link.LinkTarget.OriginalString);
                }
            }
        }

        info.Description = text.ToString();

        return info;
    }
}
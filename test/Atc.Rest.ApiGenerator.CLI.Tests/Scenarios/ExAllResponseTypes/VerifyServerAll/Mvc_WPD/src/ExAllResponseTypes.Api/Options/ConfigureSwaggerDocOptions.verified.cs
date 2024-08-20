﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace ExAllResponseTypes.Api.Options;

[GeneratedCode("ApiGenerator", "x.x.x.x")]
public class ConfigureSwaggerDocOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;
    private readonly IWebHostEnvironment environment;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigureSwaggerDocOptions"/> class.
    /// </summary>
    /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
    /// <param name="environment">The environment.</param>
    public ConfigureSwaggerDocOptions(
        IApiVersionDescriptionProvider provider,
        IWebHostEnvironment environment)
    {
        this.provider = provider;
        this.environment = environment;
    }

    public void Configure(
        SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, "xml"));
    }

    private OpenApiInfo CreateInfoForApiVersion(
        ApiVersionDescription description)
    {
        var text = new StringBuilder("Example With All Response Types Api");
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
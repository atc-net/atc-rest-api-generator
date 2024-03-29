﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api;

[GeneratedCode("ApiGenerator", "2.0.259.29354")]
public class ConfigureSwaggerDocOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider provider;

    public ConfigureSwaggerDocOptions(
        IApiVersionDescriptionProvider provider)
        => this.provider = provider;

    public void Configure(
        SwaggerGenOptions options)
    {
        foreach (var version in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(
                version.GroupName,
                new OpenApiInfo
                {
                    Version = "1.0",
                    Title = "Demo Api",
                    Description = @"Demo Api - SingleFileVersion",
                    Contact = new OpenApiContact
                    {
                        Name = "atc-net A/S",
                    },
                });
        }

        options.IncludeXmlComments(Path.ChangeExtension(GetType().Assembly.Location, "xml"));
    }
}
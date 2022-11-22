namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiExtensionExtensions
{
    public static bool? ExtractAuthenticationRequired(
        this IDictionary<string, IOpenApiExtension> extensionsSection)
    {
        if (!extensionsSection.TryGetValue(SecurityExtensionNameConstants.AuthenticationRequired, out var extensions))
        {
            return null;
        }

        if (extensions is not OpenApiBoolean openApiBoolean)
        {
            return null;
        }

        return openApiBoolean.Value;

    }

    public static IReadOnlyCollection<string> ExtractAuthorizationRoles(
        this IDictionary<string, IOpenApiExtension> extensionsSection)
        => extensionsSection.ExtractSecurityExtensions(SecurityExtensionNameConstants.AuthorizeRoles);

    public static IReadOnlyCollection<string> ExtractAuthenticationSchemes(
        this IDictionary<string, IOpenApiExtension> extensionsSection)
        => extensionsSection.ExtractSecurityExtensions(SecurityExtensionNameConstants.AuthenticationSchemes);

    private static IReadOnlyCollection<string> ExtractSecurityExtensions(
        this IDictionary<string, IOpenApiExtension> extensionsSection,
        string extensionName)
    {
        var securityExtensions = new List<string>();
        if (!extensionsSection.TryGetValue(extensionName, out var extensions))
        {
            return securityExtensions;
        }

        if (extensions is not OpenApiArray extensionsArray)
        {
            return securityExtensions;
        }

        foreach (var extension in extensionsArray)
        {
            if (extension is not OpenApiString openApiString)
            {
                continue;
            }

            if (!securityExtensions.Contains(openApiString.Value, StringComparer.OrdinalIgnoreCase))
            {
                securityExtensions.Add(openApiString.Value);
            }
        }

        return securityExtensions;
    }
}
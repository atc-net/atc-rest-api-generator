namespace Structure1.Api.Extensions;

public static class WebApplicationExtensions
{
    private static readonly string[] PatchHttpMethods = ["patch"];

    public static RouteHandlerBuilder MapPatch(
        this WebApplication app,
        string pattern,
        Delegate handler)
        => app.MapMethods(
            pattern,
            PatchHttpMethods,
            handler);
}
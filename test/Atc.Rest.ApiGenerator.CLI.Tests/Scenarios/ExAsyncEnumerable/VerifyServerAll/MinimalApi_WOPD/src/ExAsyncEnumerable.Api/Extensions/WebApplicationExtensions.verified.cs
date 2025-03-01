namespace ExAsyncEnumerable.Api.Extensions;

public static class WebApplicationExtensions
{
    [SuppressMessage("Spacing Rules", "SA1010:Opening Square Brackets Must Be Spaced Correctly", Justification = "OK.")]
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
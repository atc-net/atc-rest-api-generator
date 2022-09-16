namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators;

public class SwaggerDocOptionsTests
{
    private readonly OpenApiDocument openApiDocument;
    private readonly string code;

    public SwaggerDocOptionsTests()
    {
        openApiDocument = new OpenApiDocument
        {
            Info = new OpenApiInfo
            {
                Contact = new OpenApiContact { Email = "apiteam@swagger.io" },
                Description = @"
This is a sample Pet Store Server based on the OpenAPI 3.0 specification.  You can find out more about
Swagger at [http://swagger.io](http://swagger.io). In the third iteration of the pet store, we've switched to the design first approach!
You can now help us improve the API whether it's by making changes to the definition itself or to the code.
That way, with time, we can improve the API in general, and expose some of the new features in OAS3.

Some useful links:
- [The Pet Store repository](https://github.com/swagger-api/swagger-petstore)
- [The source API definition for the Pet Store](https://github.com/swagger-api/swagger-petstore/blob/master/src/main/resources/openapi.yaml)".EnsureEnvironmentNewLines(),
                License = new OpenApiLicense
                {
                    Name = "Apache 2.0", Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html"),
                },
                Title = "Swagger Petstore - OpenAPI 3.0",
                Version = "1.0.6",
                TermsOfService = new Uri("http://swagger.io/terms/"),
            },
        };

        var generator = new SyntaxGeneratorSwaggerDocOptions(GetType().Namespace!, openApiDocument);
        code = generator.GenerateCode();
    }

    [Fact]
    public void GeneratedCode_Should_NotBeNullOrWhitespace()
        => Assert.False(string.IsNullOrWhiteSpace(code));

    [Fact]
    public void GeneratedCode_ClassName_Should_Be_ConfigureSwaggerDocOptions()
        => Assert.Contains(
            "class ConfigureSwaggerDocOptions",
            code,
            StringComparison.Ordinal);

    [Fact]
    public void GeneratedCode_ClassName_Should_Implement_IConfigureOptions_SwaggerGenOptions()
        => Assert.Contains(
            ": IConfigureOptions<SwaggerGenOptions>",
            code,
            StringComparison.Ordinal);

    [Fact]
    public void GeneratedCode_Constructor_Accepts_IApiVersionDescriptionProvider()
        => Assert.Contains(
            "public ConfigureSwaggerDocOptions(IApiVersionDescriptionProvider provider)",
            code,
            StringComparison.Ordinal);

    [Fact]
    public void GeneratedCode_Iterates_Over_ApiVersionDescriptions()
        => Assert.Contains(
            "foreach (var version in provider.ApiVersionDescriptions)",
            code,
            StringComparison.Ordinal);

    [Fact]
    public void GeneratedCode_Creates_SwaggerDoc_For_Version_GroupName()
        => Assert.Contains(
            "options.SwaggerDoc(version.GroupName",
            code
                .RemoveNewLines()
                .Replace(" ", string.Empty, StringComparison.Ordinal),
            StringComparison.Ordinal);

    [Fact]
    public void GeneratedCode_Should_Contain_Title()
        => Assert.Contains(
            openApiDocument.Info.Title,
            code,
            StringComparison.OrdinalIgnoreCase);

    // TODO: vNext - Fix before release
    ////[Fact]
    ////public void GeneratedCode_Should_Contain_Description()
    ////    => Assert.Contains(
    ////        openApiDocument.Info.Description,
    ////        code,
    ////        StringComparison.OrdinalIgnoreCase);

    [Fact]
    public void GeneratedCode_Should_Contain_Version()
        => Assert.Contains(
            openApiDocument.Info.Version,
            code,
            StringComparison.OrdinalIgnoreCase);

    [Fact]
    public void GeneratedCode_Should_Contain_Contact_Email()
        => Assert.Contains(
            openApiDocument.Info.Contact.Email,
            code,
            StringComparison.OrdinalIgnoreCase);

    [Fact]
    public void GeneratedCode_Should_Contain_License_Name()
        => Assert.Contains(
            openApiDocument.Info.License.Name,
            code,
            StringComparison.OrdinalIgnoreCase);

    [Fact]
    public void GeneratedCode_Should_Contain_License_Url()
        => Assert.Contains(
            openApiDocument.Info.License.Url.ToString(),
            code,
            StringComparison.OrdinalIgnoreCase);

    [Fact]
    public void GeneratedCode_Should_Contain_TermsOfService()
        => Assert.Contains(
            openApiDocument.Info.TermsOfService.ToString(),
            code,
            StringComparison.OrdinalIgnoreCase);
}
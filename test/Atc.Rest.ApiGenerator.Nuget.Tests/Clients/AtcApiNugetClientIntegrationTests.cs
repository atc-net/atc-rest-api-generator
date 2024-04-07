namespace Atc.Rest.ApiGenerator.Nuget.Tests.Clients;

[Collection("Sequential-Endpoints")]
[Trait(Traits.Category, Traits.Categories.Integration)]
public class AtcApiNugetClientIntegrationTests
{
    [Theory]
    [InlineData("2.0.280", "Atc")]
    [InlineData(null, "xAtcDummy")]
    public async Task RetrieveLatestVersionForPackageId(
        string? expectedMinPackageVersion,
        string packageId)
    {
        // Arrange
        using var cts = new CancellationTokenSource();

        var sut = new AtcApiNugetClient(NullLogger<AtcApiNugetClient>.Instance);

        // Act
        var actual = await sut.RetrieveLatestVersionForPackageId(packageId, cts.Token);

        // Assert
        if (string.IsNullOrEmpty(expectedMinPackageVersion))
        {
            Assert.Null(actual);
        }
        else
        {
            Assert.NotNull(actual);
            Assert.True(actual.GreaterThanOrEqualTo(Version.Parse(expectedMinPackageVersion)));
        }
    }
}
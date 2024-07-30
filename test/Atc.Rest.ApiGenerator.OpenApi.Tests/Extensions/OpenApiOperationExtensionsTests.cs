namespace Atc.Rest.ApiGenerator.OpenApi.Tests.Extensions;

public sealed class OpenApiOperationExtensionsTests
{
    [Fact]
    public void ExtractApiOperationAuthorization_ReturnsNull_WhenNoAuthorizationRequired()
    {
        // Arrange
        var apiPath = new OpenApiPathItem();
        var apiOperation = new OpenApiOperation();

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ExtractApiOperationAuthorization_ReturnsModel_WithAuthorizationRolesForPath()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("api.execute.all") } },
            },
        };
        var apiOperation = new OpenApiOperation();

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AuthenticationSchemes);
        Assert.NotNull(result.Roles);
        Assert.Contains("api.execute.all", result.Roles);
        Assert.False(result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiOperationAuthorization_ReturnsModel_WithAuthorizationRolesForOperation()
    {
        // Arrange
        var apiPath = new OpenApiPathItem();
        var apiOperation = new OpenApiOperation
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("api.execute.all") } },
            },
        };

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AuthenticationSchemes);
        Assert.NotNull(result.Roles);
        Assert.Contains("api.execute.all", result.Roles);
        Assert.False(result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiOperationAuthorization_ReturnsModel_WithAuthenticationSchemesForPath()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme1") } },
            },
        };
        var apiOperation = new OpenApiOperation();

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AuthenticationSchemes);
        Assert.Null(result.Roles);
        Assert.Contains("scheme1", result.AuthenticationSchemes);
        Assert.False(result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiOperationAuthorization_ReturnsModel_WithAuthenticationSchemesForOperation()
    {
        // Arrange
        var apiPath = new OpenApiPathItem();
        var apiOperation = new OpenApiOperation
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme1") } },
            },
        };

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AuthenticationSchemes);
        Assert.Null(result.Roles);
        Assert.Contains("scheme1", result.AuthenticationSchemes);
        Assert.False(result.UseAllowAnonymous);
    }

    [Theory]
    [InlineData(false, false, true)]
    [InlineData(true, false, true)]
    [InlineData(false, true, false)]
    [InlineData(true, true, false)]
    public void ExtractApiOperationAuthorization_ReturnsModel_WithCorrectAllowAnonymous(
        bool pathAuthenticationRequired,
        bool operationAuthenticationRequired,
        bool expectedUseAllowAnonymous)
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthenticationRequired, new OpenApiBoolean(pathAuthenticationRequired) },
            },
        };
        var apiOperation = new OpenApiOperation
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthenticationRequired, new OpenApiBoolean(operationAuthenticationRequired) },
            },
        };

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AuthenticationSchemes);
        Assert.Null(result.Roles);
        Assert.Equal(expectedUseAllowAnonymous, result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiOperationAuthorization_ReturnsModel_WithMultipleRolesAndAuthenticationSchemes()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("role1"), new OpenApiString("role2") } },
                { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme1"), new OpenApiString("scheme2") } },
            },
        };
        var apiOperation = new OpenApiOperation
        {
            Extensions = new Dictionary<string, IOpenApiExtension>
            {
                { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("role3"), new OpenApiString("role4") } },
                { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme3"), new OpenApiString("scheme4") } },
            },
        };

        // Act
        var result = apiOperation.ExtractApiOperationAuthorization(apiPath);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Roles);
        Assert.Contains("role1", result.Roles);
        Assert.Contains("role2", result.Roles);
        Assert.Contains("role3", result.Roles);
        Assert.Contains("role4", result.Roles);
        Assert.NotNull(result.AuthenticationSchemes);
        Assert.Contains("scheme1", result.AuthenticationSchemes);
        Assert.Contains("scheme2", result.AuthenticationSchemes);
        Assert.Contains("scheme3", result.AuthenticationSchemes);
        Assert.Contains("scheme4", result.AuthenticationSchemes);
        Assert.False(result.UseAllowAnonymous);
    }
}
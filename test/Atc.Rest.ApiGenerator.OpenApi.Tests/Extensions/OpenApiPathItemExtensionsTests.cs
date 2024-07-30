namespace Atc.Rest.ApiGenerator.OpenApi.Tests.Extensions;

public sealed class OpenApiPathItemExtensionsTests
{
    [Fact]
    public void ExtractApiPathAuthorization_ReturnsNull_WhenNoOperationsPresent()
    {
        // Arrange
        var apiPath = new OpenApiPathItem();

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ExtractApiPathAuthorization_ReturnsModel_WithAuthorizationRolesForOperations()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("api.execute.all") } },
                        },
                    }
                },
            },
        };

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AuthenticationSchemes);
        Assert.NotNull(result.Roles);
        Assert.Contains("api.execute.all", result.Roles);
        Assert.False(result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiPathAuthorization_ReturnsModel_WithAuthenticationSchemesForOperations()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme1") } },
                        },
                    }
                },
            },
        };

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AuthenticationSchemes);
        Assert.Null(result.Roles);
        Assert.Contains("scheme1", result.AuthenticationSchemes);
        Assert.False(result.UseAllowAnonymous);
    }

    [Theory]
    [InlineData(false, false, null)]
    [InlineData(true, false, false)]
    [InlineData(false, true, null)]
    [InlineData(true, true, false)]
    public void ExtractApiPathAuthorization(
        bool pathAuthenticationRequired,
        bool operationAuthenticationRequired,
        bool? expectedUseAllowAnonymous)
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthenticationRequired, new OpenApiBoolean(operationAuthenticationRequired) },
                        },
                    }
                },
            },
            Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
            {
                { SecurityExtensionNameConstants.AuthenticationRequired, new OpenApiBoolean(pathAuthenticationRequired) },
            },
        };

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

        // Assert
        if (expectedUseAllowAnonymous is null)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.NotNull(result);
            Assert.Null(result.AuthenticationSchemes);
            Assert.Null(result.Roles);
            Assert.Equal(expectedUseAllowAnonymous, result.UseAllowAnonymous);
        }
    }

    [Fact]
    public void ExtractApiPathAuthorization_MergesRolesFromMultipleOperations()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("role1") } },
                        },
                    }
                },
                {
                    OperationType.Post, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("role2") } },
                        },
                    }
                },
            },
        };

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.AuthenticationSchemes);
        Assert.NotNull(result.Roles);
        Assert.Contains("role1", result.Roles);
        Assert.Contains("role2", result.Roles);
        Assert.False(result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiPathAuthorization_MergesAuthenticationSchemesFromMultipleOperations()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme1") } },
                        },
                    }
                },
                {
                    OperationType.Post, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme2") } },
                        },
                    }
                },
            },
        };

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.AuthenticationSchemes);
        Assert.Null(result.Roles);
        Assert.Contains("scheme1", result.AuthenticationSchemes);
        Assert.Contains("scheme2", result.AuthenticationSchemes);
        Assert.False(result.UseAllowAnonymous);
    }

    [Fact]
    public void ExtractApiPathAuthorization_ReturnsModel_WithMultipleRolesAndAuthenticationSchemesForPathAndOperations()
    {
        // Arrange
        var apiPath = new OpenApiPathItem
        {
            Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
            {
                { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("role1"), new OpenApiString("role2") } },
                { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme1"), new OpenApiString("scheme2") } },
            },
            Operations = new Dictionary<OperationType, OpenApiOperation>
            {
                {
                    OperationType.Get, new OpenApiOperation
                    {
                        Extensions = new Dictionary<string, IOpenApiExtension>(StringComparer.Ordinal)
                        {
                            { SecurityExtensionNameConstants.AuthorizeRoles, new OpenApiArray { new OpenApiString("role3"), new OpenApiString("role4") } },
                            { SecurityExtensionNameConstants.AuthenticationSchemes, new OpenApiArray { new OpenApiString("scheme3"), new OpenApiString("scheme4") } },
                        },
                    }
                },
            },
        };

        // Act
        var result = apiPath.ExtractApiPathAuthorization();

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
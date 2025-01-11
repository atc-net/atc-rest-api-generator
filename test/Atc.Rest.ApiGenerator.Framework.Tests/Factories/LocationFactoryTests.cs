namespace Atc.Rest.ApiGenerator.Framework.Tests.Factories;

public class LocationFactoryTests
{
    [Theory]
    [InlineData("MyProject.Folder.[[apiGroupName]].MyUser", "Users", "MyProject.Folder.Users.MyUser")]
    [InlineData("MyProject.[[apiGroupName]].MyUser", "Users", "MyProject.Users.MyUser")]
    [InlineData("[[apiGroupName]].MyUser", "Users", "Users.MyUser")]
    public void CreateWithApiGroupName_ShouldReturnCorrect_Location(
        string value,
        string apiGroupName,
        string expected)
    {
        // Act
        var result = LocationFactory.CreateWithApiGroupName(apiGroupName, value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("MyProject.Folder.TemplateForApiGroupName.MyUser", "MyProject.Folder.TemplateForApiGroupName.MyUser")]
    [InlineData("MyProject.TemplateForApiGroupName.MyUser", "MyProject.TemplateForApiGroupName.MyUser")]
    [InlineData("TemplateForApiGroupName.MyUser", "TemplateForApiGroupName.MyUser")]
    public void CreateWithoutTemplateForApiGroupName_ShouldRemoveTemplateKey(
        string value,
        string expected)
    {
        // Act
        var result = LocationFactory.CreateWithoutTemplateForApiGroupName(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CreateWithoutTemplateForApiGroupName_ShouldReturnEmpty_WhenValueIsNull()
    {
        // Act
        var result = LocationFactory.CreateWithoutTemplateForApiGroupName(null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData(new[] { "MyProject", "folder", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", "nested.folder", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", @"nested\folder", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", ".", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Users.MyUser")]
    [InlineData(new[] { "MyProject", ".", "MyUser" }, "Users", "MyProject.MyUser.Users")]
    public void Create_WithTemplateItems_ShouldReturnCorrect_Location(
        string[] values,
        string apiGroupName,
        string expected)
    {
        // Act
        var result = LocationFactory.Create(
            new List<KeyValueItem> { new("apiGroupName", apiGroupName) },
            values);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Create_WithNullValues_ShouldReturnEmptyString()
    {
        // Act
        var result = LocationFactory.Create(
            new List<KeyValueItem> { new("apiGroupName", "Users") },
            null!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Create_WithEmptyTemplateItems_ShouldReturnJoinedValues()
    {
        // Arrange
        var values = new[] { "MyProject", "folder", "MyUser" };

        // Act
        var result = LocationFactory.Create(new List<KeyValueItem>(), values);

        // Assert
        Assert.Equal("MyProject.Folder.MyUser", result);
    }
}
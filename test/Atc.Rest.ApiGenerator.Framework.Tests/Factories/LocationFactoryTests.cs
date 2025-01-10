namespace Atc.Rest.ApiGenerator.Framework.Tests.Factories;

public class LocationFactoryTests
{
    [Theory]
    [InlineData(new string[] { }, "")]
    [InlineData(new[] { "MyNamespace" }, "MyNamespace")]
    [InlineData(new[] { "MyNamespace", "SubNamespace" }, "MyNamespace.SubNamespace")]
    [InlineData(new[] { "MyProject" }, "MyProject")]
    [InlineData(new[] { "", "SubNamespace1" }, "SubNamespace1")]
    [InlineData(new[] { "MyProject", "SubNamespace1" }, "MyProject.SubNamespace1")]
    [InlineData(new[] { "MyProject", "SubNamespace1", "SubNamespace2" }, "MyProject.SubNamespace1.SubNamespace2")]
    [InlineData(new[] { "MyProject", "folder", "Users", "MyUser" }, "MyProject.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", "nested.folder", "Users", "MyUser" }, "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", @"nested\folder", "Users", "MyUser" }, "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject",  ".", "Users", "MyUser" }, "MyProject.Users.MyUser")]
    public void Create_ShouldReturnCorrectNamespace(
        string[] namespaceParts,
        string expected)
    {
        // Act
        var result = LocationFactory.Create(namespaceParts);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new[] { "MyProject", "folder", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", "nested.folder", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", @"nested\folder", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData(new[] { "MyProject", ".", "[[apiGroupName]]", "MyUser" }, "Users", "MyProject.Users.MyUser")]
    [InlineData(new[] { "MyProject", ".", "MyUser" }, "Users", "MyProject.MyUser.Users")]
    public void Create_WithTemplateItems_ShouldReturnCorrectNamespace(
        string[] namespaceParts,
        string apiGroupName,
        string expected)
    {
        // Act
        var result = LocationFactory.Create(
            new List<KeyValueItem> { new("apiGroupName", apiGroupName) },
            namespaceParts);

        // Assert
        Assert.Equal(expected, result);
    }
}
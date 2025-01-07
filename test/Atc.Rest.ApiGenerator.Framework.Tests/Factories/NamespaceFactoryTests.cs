namespace Atc.Rest.ApiGenerator.Framework.Tests.Factories;

public class NamespaceFactoryTests
{
    [Theory]
    [InlineData("MyProject", new string[] { }, "MyProject")]
    [InlineData("", new[] { "SubNamespace1" }, "SubNamespace1")]
    [InlineData("MyProject", new[] { "SubNamespace1" }, "MyProject.SubNamespace1")]
    [InlineData("MyProject", new[] { "SubNamespace1", "SubNamespace2" }, "MyProject.SubNamespace1.SubNamespace2")]
    [InlineData("MyProject", new[] { "folder", "Users", "MyUser" }, "MyProject.Folder.Users.MyUser")]
    [InlineData("MyProject", new[] { "nested.folder", "Users", "MyUser" }, "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData("MyProject", new[] { @"nested\folder", "Users", "MyUser" }, "MyProject.Nested.Folder.Users.MyUser")]
    [InlineData("MyProject", new[] { ".", "Users", "MyUser" }, "MyProject.Users.MyUser")]
    public void CreateFull_ShouldReturnCorrectNamespace(
        string projectName,
        string[] subNamespaces,
        string expected)
    {
        // Act
        var result = NamespaceFactory.CreateFull(projectName, subNamespaces);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(new string[] { }, "")]
    [InlineData(new[] { "MyNamespace" }, "MyNamespace")]
    [InlineData(new[] { "MyNamespace", "SubNamespace" }, "MyNamespace.SubNamespace")]
    public void Create_ShouldReturnCorrectNamespace(
        string[] subNamespaces,
        string expected)
    {
        // Act
        var result = NamespaceFactory.Create(subNamespaces);

        // Assert
        Assert.Equal(expected, result);
    }
}
namespace Atc.Rest.ApiGenerator.Framework.Tests.Factories;

public class NamespaceFactoryTests
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
    [InlineData(new[] { "MyProject", ".", "Users", "MyUser" }, "MyProject.Users.MyUser")]
    [InlineData(new[] { "IoT", "SubNamespace" }, "IoT.SubNamespace")]
    [InlineData(new[] { "hallo-world", "SubNamespace" }, "HalloWorld.SubNamespace")]
    [InlineData(new[] { "AAA", "SubNamespace" }, "AAA.SubNamespace")]
    public void Create_ShouldReturnCorrect_Namespace(
        string[] values,
        string expected)
    {
        // Act
        var result = NamespaceFactory.Create(values);

        // Assert
        Assert.Equal(expected, result);
    }
}
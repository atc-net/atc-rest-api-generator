namespace Atc.Rest.ApiGenerator.Framework.Tests.Factories;

public class FileInfoFactoryTests
{
    [Theory]
    [InlineData(@"C:\Project", new[] { "SubFolder", "File.cs" }, @"C:\Project\SubFolder\File.cs")]
    [InlineData(@"C:\Project", new[] { "Folder-With-Dashes", "File.cs" }, @"C:\Project\Folder\With\Dashes\File.cs")]
    [InlineData(@"C:\Project", new[] { "nested.folder", "File.cs" }, @"C:\Project\Nested\Folder\File.cs")]
    [InlineData(@"C:\Project", new[] { "_shared", "File.cs" }, @"C:\Project\_Shared\File.cs")]
    [InlineData(@"C:\Project", new[] { "_enumerationTypes", "File.cs" }, @"C:\Project\_EnumerationTypes\File.cs")]
    [InlineData(@"C:\Project", new[] { "folder", "Users", "MyUser.cs" }, @"C:\Project\Folder\Users\MyUser.cs")]
    [InlineData(@"C:\Project", new[] { "nested.folder", "Users", "MyUser.cs" }, @"C:\Project\Nested\Folder\Users\MyUser.cs")]
    [InlineData(@"C:\Project", new[] { @"nested\folder", "Users", "MyUser.cs" }, @"C:\Project\Nested\Folder\Users\MyUser.cs")]
    [InlineData(@"C:\Project", new[] { ".", "Users", "MyUser.cs" }, @"C:\Project\Users\MyUser.cs")]
    public void Create_ShouldReturnCorrectFileInfo(
        string projectPath,
        string[] subParts,
        string expectedFilePath)
    {
        // Arrange
        var directoryInfo = new DirectoryInfo(projectPath);

        // Act
        var fileInfo = FileInfoFactory.Create(directoryInfo, subParts);

        // Assert
        Assert.Equal(expectedFilePath, fileInfo.FullName);
    }

    [Theory]
    [InlineData(@"C:\Project", new string[] { })]
    [InlineData(@"C:\Project", new[] { "SubFolder", "FileWithoutExtension" })]
    public void Create_ShouldThrowException(
        string projectPath,
        string[] subParts)
    {
        // Arrange
        var directoryInfo = new DirectoryInfo(projectPath);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => FileInfoFactory.Create(directoryInfo, subParts));
    }
}
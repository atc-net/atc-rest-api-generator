namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerGenerator : IServerGenerator
{
    private readonly ILogger<ServerGenerator> logger;
    private readonly string projectName;
    private readonly DirectoryInfo rootPath;
    private readonly DirectoryInfo srcPath;
    private readonly DirectoryInfo? testPath;

    public ServerGenerator(
        ILoggerFactory loggerFactory,
        string projectName,
        DirectoryInfo rootPath,
        DirectoryInfo srcPath,
        DirectoryInfo? testPath)
    {
        ArgumentNullException.ThrowIfNull(loggerFactory);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(rootPath);
        ArgumentNullException.ThrowIfNull(srcPath);

        logger = loggerFactory.CreateLogger<ServerGenerator>();
        this.projectName = projectName;
        this.rootPath = rootPath;
        this.srcPath = srcPath;
        this.testPath = testPath;
    }

    public void ScaffoldSolutionFile()
    {
        var content = GenerateContentForSolutionFileHelper.Generate(
            projectName,
            rootPath,
            srcPath,
            testPath);

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            rootPath,
            rootPath.CombineFileInfo($"{projectName}.sln"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }

    public void ScaffoldSolutionDotSettingsFile()
    {
        var parameters = new SolutionDotSettingsFileParameters();

        var contentGenerator = new GenerateContentForSolutionDotSettingsFile(
            parameters);

        var content = contentGenerator.Generate();

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            rootPath,
            rootPath.CombineFileInfo($"{projectName}.sln.DotSettings"),
            ContentWriterArea.Src,
            content,
            overrideIfExist: false);
    }
}
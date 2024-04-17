namespace Atc.Rest.ApiGenerator.Framework.ProjectGenerator;

public interface IServerApiGenerator
{
    void GeneratedAssemblyMarker(
        ILogger logger,
        string projectName,
        Version projectVersion,
        DirectoryInfo path);
}
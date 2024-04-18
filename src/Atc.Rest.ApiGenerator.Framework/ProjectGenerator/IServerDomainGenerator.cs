namespace Atc.Rest.ApiGenerator.Framework.ProjectGenerator;

public interface IServerDomainGenerator
{
    void GeneratedAssemblyMarker(
        ILogger logger,
        string projectName,
        Version projectVersion,
        DirectoryInfo path);

    void GenerateCollectionExtensions(
        ILogger logger,
        string projectName,
        Version projectVersion,
        DirectoryInfo path,
        OpenApiDocument openApiDocument);
}
namespace Atc.Rest.ApiGenerator.Framework.ProjectGenerator;

/// <summary>
/// Interface: IServerDomainGenerator
/// </summary>
/// <remarks>
/// Method prefixes:
/// - Generate => Generate file and overwrite file if exists.
/// - Scaffold => Generate file if not exists.
/// - Maintain => Update file.
/// </remarks>
public interface IServerDomainGenerator
{
    Task ScaffoldProjectFile();

    void GenerateAssemblyMarker();

    void GenerateServiceCollectionExtensions();

    void GenerateHandlers();

    void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings);
}
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
    void GenerateAssemblyMarker();

    void GenerateServiceCollectionExtensions();

    void MaintainGlobalUsings(
        string apiProjectName,
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        IList<ApiOperation> operationSchemaMappings);
}
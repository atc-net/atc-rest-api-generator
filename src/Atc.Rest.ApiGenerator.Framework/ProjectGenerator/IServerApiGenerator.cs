namespace Atc.Rest.ApiGenerator.Framework.ProjectGenerator;

/// <summary>
/// Interface: IServerApiGenerator
/// </summary>
/// <remarks>
/// Method prefixes:
/// - Generate => Generate file and overwrite file if exists.
/// - Scaffold => Generate file if not exists.
/// - Maintain => Update file.
/// </remarks>
public interface IServerApiGenerator
{
    void ScaffoldProjectFile();

    void GenerateAssemblyMarker();

    void GenerateModels();

    void GenerateParameters();

    void GenerateResults();

    void GenerateInterfaces();

    void GenerateEndpoints();

    void MaintainApiSpecification(
        FileInfo apiSpecificationFile);

    void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings);
}
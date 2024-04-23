namespace Atc.Rest.ApiGenerator.Framework.ProjectGenerator;

/// <summary>
/// Interface: IServerHostGenerator
/// </summary>
/// <remarks>
/// Method prefixes:
/// - Generate => Generate file and overwrite file if exists.
/// - Scaffold => Generate file if not exists.
/// - Maintain => Update file.
/// </remarks>
public interface IServerHostGenerator
{
    bool UseRestExtended { get; set; }

    void ScaffoldProgramFile();

    void ScaffoldStartupFile();

    void ScaffoldWebConfig();

    void ScaffoldConfigureSwaggerDocOptions();

    void ScaffoldServiceCollectionExtensions();

    void ScaffoldServiceWebApplicationExtensions(
        SwaggerThemeMode swaggerThemeMode);

    void ScaffoldConfigureSwaggerOptions();

    void MaintainGlobalUsings(
        string domainProjectName,
        IList<string> apiGroupNames,
        bool removeNamespaceGroupSeparatorInGlobalUsings);

    void MaintainWwwResources();
}
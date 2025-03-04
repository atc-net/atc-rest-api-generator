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

    Task ScaffoldProjectFile();

    void ScaffoldPropertiesLaunchSettingsFile();

    void ScaffoldJsonSerializerOptionsExtensions();

    void ScaffoldServiceCollectionExtensions();

    void ScaffoldWebApplicationBuilderExtensions();

    void ScaffoldWebApplicationExtensions(
        SwaggerThemeMode swaggerThemeMode);

    void GenerateConfigureSwaggerDocOptions();

    void ScaffoldProgramFile(
        SwaggerThemeMode swaggerThemeMode);

    void ScaffoldStartupFile();

    void ScaffoldWebConfig();

    void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings,
        bool usingCodingRules);

    void MaintainWwwResources();
}
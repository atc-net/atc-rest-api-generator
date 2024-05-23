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
public interface IServerHostTestGenerator
{
    Task ScaffoldProjectFile();

    void ScaffoldAppSettingsIntegrationTestFile();

    void GenerateWebApiStartupFactoryFile();

    void GenerateWebApiControllerBaseTestFile();

    void GenerateEndpointHandlerStubs();

    void GenerateEndpointTests();

    void MaintainGlobalUsings(
        bool usingCodingRules,
        bool removeNamespaceGroupSeparatorInGlobalUsings);
}
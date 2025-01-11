namespace Atc.Rest.ApiGenerator.Client.CSharp.ProjectGenerator;

/// <summary>
/// Interface: IClientCSharpApiGenerator
/// </summary>
/// <remarks>
/// Method prefixes:
/// - Generate => Generate file and overwrite file if exists.
/// - Scaffold => Generate file if not exists.
/// - Maintain => Update file.
/// </remarks>
public interface IClientCSharpApiGenerator
{
    string HttpClientName { get; set; }

    Task ScaffoldProjectFile();

    void GenerateModels();

    void GenerateParameters();

    void GenerateEndpointInterfaces();

    void GenerateEndpoints();

    void GenerateEndpointResultInterfaces();

    void GenerateEndpointResults();

    void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings);
}
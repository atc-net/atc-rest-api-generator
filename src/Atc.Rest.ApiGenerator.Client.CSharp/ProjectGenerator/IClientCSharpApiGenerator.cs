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
    string? ClientFolderName { get; set; }

    string HttpClientName { get; set; }

    bool UseProblemDetailsAsDefaultBody { get; set; }

    void ScaffoldProjectFile();

    void GenerateModels();

    void GenerateParameters();

    void GenerateEndpointInterfaces();

    void GenerateEndpoints();

    void GenerateEndpointResultInterfaces();

    void GenerateEndpointResults();

    void MaintainGlobalUsings(
        bool removeNamespaceGroupSeparatorInGlobalUsings);
}
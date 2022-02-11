namespace Atc.Rest.ApiGenerator.Models;

public class ApiProjectOptions : BaseProjectOptions
{
    public ApiProjectOptions(
        DirectoryInfo projectSrcGeneratePath,
        DirectoryInfo? projectTestGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        string projectSuffixName,
        ApiOptions.ApiOptions apiOptions,
        bool usingCodingRules,
        bool forClient = false,
        string? clientFolderName = null)
        : base(
            projectSrcGeneratePath,
            projectTestGeneratePath,
            openApiDocument,
            openApiDocumentFile,
            projectPrefixName,
            projectSuffixName,
            apiOptions,
            usingCodingRules,
            forClient,
            clientFolderName)
    {
        if (string.IsNullOrEmpty(clientFolderName))
        {
            PathForEndpoints = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, NameConstants.Endpoints));
            PathForContracts = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, NameConstants.Contracts));
            PathForContractsShared = new DirectoryInfo(Path.Combine(PathForContracts.FullName, NameConstants.ContractsSharedModels));
        }
        else
        {
            PathForEndpoints = new DirectoryInfo(Path.Combine(Path.Combine(PathForSrcGenerate.FullName, clientFolderName), NameConstants.Endpoints));
            PathForContracts = new DirectoryInfo(Path.Combine(Path.Combine(PathForSrcGenerate.FullName, clientFolderName), NameConstants.Contracts));
            PathForContractsShared = new DirectoryInfo(Path.Combine(Path.Combine(PathForContracts.FullName), NameConstants.ContractsSharedModels));
        }
    }

    public DirectoryInfo PathForEndpoints { get; }

    public DirectoryInfo PathForContracts { get; }

    public DirectoryInfo PathForContractsShared { get; }
}
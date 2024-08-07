namespace Atc.Rest.ApiGenerator.Models;

public class ApiProjectOptions : BaseProjectOptions
{
    public ApiProjectOptions(
        DirectoryInfo projectSrcGeneratePath,
        DirectoryInfo? projectTestGeneratePath,
        OpenApiDocument openApiDocument,
        FileInfo openApiDocumentFile,
        string projectPrefixName,
        string? projectSuffixName,
        ApiOptions apiOptions,
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
            PathForEndpoints = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ContentGeneratorConstants.Endpoints));
            PathForContracts = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, ContentGeneratorConstants.Contracts));
            PathForContractsShared = new DirectoryInfo(Path.Combine(PathForContracts.FullName, ContentGeneratorConstants.SpecialFolderSharedModels));
        }
        else
        {
            PathForEndpoints = new DirectoryInfo(Path.Combine(Path.Combine(PathForSrcGenerate.FullName, clientFolderName), ContentGeneratorConstants.Endpoints));
            PathForContracts = new DirectoryInfo(Path.Combine(Path.Combine(PathForSrcGenerate.FullName, clientFolderName), ContentGeneratorConstants.Contracts));
            PathForContractsShared = new DirectoryInfo(Path.Combine(Path.Combine(PathForContracts.FullName), ContentGeneratorConstants.SpecialFolderSharedModels));
        }
    }

    public DirectoryInfo PathForEndpoints { get; }

    public DirectoryInfo PathForContracts { get; }

    public DirectoryInfo PathForContractsShared { get; }
}
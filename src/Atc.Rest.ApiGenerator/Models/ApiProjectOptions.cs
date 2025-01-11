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
        bool forClient = false)
        : base(
            projectSrcGeneratePath,
            projectTestGeneratePath,
            openApiDocument,
            openApiDocumentFile,
            projectPrefixName,
            projectSuffixName,
            apiOptions,
            usingCodingRules,
            forClient)
    {
    }
}
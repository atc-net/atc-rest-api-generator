using System.IO;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Models
{
    public class ApiProjectOptions : BaseProjectOptions
    {
        public ApiProjectOptions(
            DirectoryInfo projectSrcGeneratePath,
            DirectoryInfo? projectTestGeneratePath,
            OpenApiDocument openApiDocument,
            FileInfo openApiDocumentFile,
            string projectPrefixName,
            ApiOptions.ApiOptions apiOptions)
            : base(
                projectSrcGeneratePath,
                projectTestGeneratePath,
                openApiDocument,
                openApiDocumentFile,
                projectPrefixName,
                "Api.Generated",
                apiOptions)
        {
            PathForEndpoints = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, NameConstants.Endpoints));
            PathForContracts = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, NameConstants.Contracts));
            PathForContractsEnumerationTypes = new DirectoryInfo(Path.Combine(PathForContracts.FullName, NameConstants.ContractsEnumerationTypes));
            PathForContractsShared = new DirectoryInfo(Path.Combine(PathForContracts.FullName, NameConstants.ContractsSharedModels));
        }

        public DirectoryInfo PathForEndpoints { get; }

        public DirectoryInfo PathForContracts { get; }

        public DirectoryInfo PathForContractsEnumerationTypes { get; }

        public DirectoryInfo PathForContractsShared { get; }
    }
}
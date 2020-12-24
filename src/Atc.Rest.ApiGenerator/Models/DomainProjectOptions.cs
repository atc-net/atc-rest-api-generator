using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Helpers;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.Models
{
    public class DomainProjectOptions : BaseProjectOptions
    {
        public DomainProjectOptions(
            DirectoryInfo projectSrcGeneratePath,
            DirectoryInfo? projectTestGeneratePath,
            OpenApiDocument openApiDocument,
            FileInfo openApiDocumentFile,
            string projectPrefixName,
            ApiOptions.ApiOptions apiOptions,
            DirectoryInfo apiProjectSrcPath)
            : base(
                projectSrcGeneratePath,
                projectTestGeneratePath,
                openApiDocument,
                openApiDocumentFile,
                projectPrefixName,
                "Domain",
                apiOptions)
        {
            ApiProjectSrcPath = apiProjectSrcPath ?? throw new ArgumentNullException(nameof(apiProjectSrcPath));
            PathForSrcHandlers = new DirectoryInfo(Path.Combine(PathForSrcGenerate.FullName, NameConstants.Handlers));
            if (PathForTestGenerate != null)
            {
                PathForTestHandlers = new DirectoryInfo(Path.Combine(PathForTestGenerate.FullName, NameConstants.Handlers));
            }
        }

        public DirectoryInfo ApiProjectSrcPath { get; private set; }

        public FileInfo? ApiProjectSrcCsProj { get; private set; }

        public DirectoryInfo? PathForSrcHandlers { get; }

        public DirectoryInfo? PathForTestHandlers { get; }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        public List<LogKeyValueItem> SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles()
        {
            var logItems = new List<LogKeyValueItem>();
            if (ApiProjectSrcPath.Exists)
            {
                var files = Directory.GetFiles(ApiProjectSrcPath.FullName, "ApiRegistration.cs", SearchOption.AllDirectories);
                if (files.Length == 1)
                {
                    ApiProjectSrcPath = new FileInfo(files[0]).Directory!;
                    files = Directory.GetFiles(ApiProjectSrcPath.FullName, "*.csproj", SearchOption.AllDirectories);
                    if (files.Length == 1)
                    {
                        ApiProjectSrcCsProj = new FileInfo(files[0]);
                    }
                }
            }

            if (ApiProjectSrcCsProj == null || !ApiProjectSrcCsProj.Exists)
            {
                logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.ProjectHostGenerated04, "Can't find API .csproj file"));
            }

            return logItems;
        }
    }
}
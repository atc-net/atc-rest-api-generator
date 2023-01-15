<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Helpers

<br />

## AtcApiNugetClientHelper

>```csharp
>public static class AtcApiNugetClientHelper
>```

### Static Methods

#### GetLatestVersionForPackageId
>```csharp
>Version GetLatestVersionForPackageId(string packageId, CancellationToken cancellationToken = null)
>```
#### GetLatestVersionForPackageId
>```csharp
>Version GetLatestVersionForPackageId(ILogger logger, string packageId, CancellationToken cancellationToken = null)
>```

<br />

## DirectoryInfoHelper

>```csharp
>public static class DirectoryInfoHelper
>```

### Static Methods

#### GetCsFileNameForContract
>```csharp
>string GetCsFileNameForContract(DirectoryInfo pathForContracts, string area, string modelName)
>```
#### GetCsFileNameForContract
>```csharp
>string GetCsFileNameForContract(DirectoryInfo pathForContracts, string area, string subArea, string modelName)
>```
#### GetCsFileNameForContractEnumTypes
>```csharp
>string GetCsFileNameForContractEnumTypes(DirectoryInfo pathForContracts, string modelName)
>```
#### GetCsFileNameForContractShared
>```csharp
>string GetCsFileNameForContractShared(DirectoryInfo pathForContracts, string modelName)
>```
#### GetCsFileNameForEndpoints
>```csharp
>string GetCsFileNameForEndpoints(DirectoryInfo pathForEndpoints, string modelName)
>```
#### GetCsFileNameForHandler
>```csharp
>string GetCsFileNameForHandler(DirectoryInfo pathForHandlers, string area, string handlerName)
>```
#### GetProjectPath
>```csharp
>DirectoryInfo GetProjectPath()
>```

<br />

## GenerateAtcCodingRulesHelper

>```csharp
>public static class GenerateAtcCodingRulesHelper
>```

### Static Fields

#### FileNameDirectoryBuildProps
>```csharp
>string FileNameDirectoryBuildProps
>```
#### FileNameEditorConfig
>```csharp
>string FileNameEditorConfig
>```
### Static Methods

#### Generate
>```csharp
>bool Generate(ILogger logger, string outputSlnPath, DirectoryInfo outputSrcPath, DirectoryInfo outputTestPath)
>```

<br />

## GenerateHelper

>```csharp
>public static class GenerateHelper
>```

### Static Methods

#### GenerateServerApi
>```csharp
>bool GenerateServerApi(ILogger logger, IApiOperationExtractor apiOperationExtractor, string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool useCodingRules)
>```
#### GenerateServerCSharpClient
>```csharp
>bool GenerateServerCSharpClient(ILogger logger, IApiOperationExtractor apiOperationExtractor, string projectPrefixName, string clientFolderName, DirectoryInfo outputPath, OpenApiDocumentContainer apiDocumentContainer, bool excludeEndpointGeneration, ApiOptions apiOptions, bool useCodingRules)
>```
#### GenerateServerDomain
>```csharp
>bool GenerateServerDomain(ILogger logger, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool useCodingRules, DirectoryInfo apiPath)
>```
#### GenerateServerHost
>```csharp
>bool GenerateServerHost(ILogger logger, IApiOperationExtractor apiOperationExtractor, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool usingCodingRules, DirectoryInfo apiPath, DirectoryInfo domainPath)
>```
#### GenerateServerSln
>```csharp
>bool GenerateServerSln(ILogger logger, string projectPrefixName, string outputSlnPath, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath)
>```
#### GetAtcApiGeneratorVersion
>```csharp
>Version GetAtcApiGeneratorVersion()
>```
#### GetAtcApiGeneratorVersionAsString3
>```csharp
>string GetAtcApiGeneratorVersionAsString3()
>```
#### GetAtcApiGeneratorVersionAsString4
>```csharp
>string GetAtcApiGeneratorVersionAsString4()
>```
#### GetAtcVersion
>```csharp
>Version GetAtcVersion()
>```
#### GetAtcVersionAsString3
>```csharp
>string GetAtcVersionAsString3()
>```
#### GetAtcVersionAsString4
>```csharp
>string GetAtcVersionAsString4()
>```

<br />

## GlobalUsingsHelper

>```csharp
>public static class GlobalUsingsHelper
>```

### Static Methods

#### CreateOrUpdate
>```csharp
>void CreateOrUpdate(ILogger logger, ContentWriterArea contentWriterArea, DirectoryInfo directoryInfo, List<string> requiredUsings)
>```

<br />

## NugetPackageReferenceHelper

>```csharp
>public static class NugetPackageReferenceHelper
>```

### Static Methods

#### CreateForApiProject
>```csharp
>IList<Tuple<string, string, string>> CreateForApiProject()
>```
#### CreateForClientApiProject
>```csharp
>IList<Tuple<string, string, string>> CreateForClientApiProject()
>```
#### CreateForHostProject
>```csharp
>IList<Tuple<string, string, string>> CreateForHostProject(bool useRestExtended)
>```
#### CreateForTestProject
>```csharp
>IList<Tuple<string, string, string>> CreateForTestProject(bool useMvc)
>```

<br />

## SolutionAndProjectHelper

>```csharp
>public static class SolutionAndProjectHelper
>```

### Static Methods

#### EnsureLatestPackageReferencesVersionInProjFile
>```csharp
>bool EnsureLatestPackageReferencesVersionInProjFile(ILogger logger, FileInfo projectCsProjFile, string fileDisplayLocation, ProjectType projectType, bool isTestProject)
>```
#### ScaffoldProjFile
>```csharp
>void ScaffoldProjFile(ILogger logger, FileInfo projectCsProjFile, string fileDisplayLocation, ProjectType projectType, bool createAsWeb, bool createAsTestProject, string projectName, string targetFramework, IList<string> frameworkReferences, IList<Tuple<string, string, string>> packageReferences, IList<FileInfo> projectReferences, bool includeApiSpecification, bool usingCodingRules)
>```
#### ScaffoldSlnFile
>```csharp
>void ScaffoldSlnFile(ILogger logger, FileInfo slnFile, string projectName, DirectoryInfo apiPath, DirectoryInfo domainPath, DirectoryInfo hostPath, DirectoryInfo domainTestPath = null, DirectoryInfo hostTestPath = null)
>```

<br />

## TrailingCharType

>```csharp
>public enum TrailingCharType
>```


| Value | Name | Description | Summary | 
| --- | --- | --- | --- | 
| 0 | None | None |  | 
| 1 | Comma | Comma |  | 
| 2 | SemiColon | Semi Colon |  | 
| 3 | Colon | Colon |  | 


<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

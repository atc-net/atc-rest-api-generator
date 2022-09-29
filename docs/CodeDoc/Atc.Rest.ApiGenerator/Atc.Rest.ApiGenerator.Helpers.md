<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Helpers

<br />

## ApiGeneratorHelper

>```csharp
>public static class ApiGeneratorHelper
>```

### Static Methods

#### CollectMissingContractModelFromOperationSchemaMappings
>```csharp
>void CollectMissingContractModelFromOperationSchemaMappings(ILogger logger, ApiProjectOptions projectOptions, List<ApiOperationSchemaMap> operationSchemaMappings, List<SyntaxGeneratorContractModel> sgContractModels)
>```

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
>bool GenerateServerApi(ILogger logger, string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptions apiOptions, bool useCodingRules)
>```
#### GenerateServerCSharpClient
>```csharp
>bool GenerateServerCSharpClient(ILogger logger, string projectPrefixName, string clientFolderName, DirectoryInfo outputPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, bool excludeEndpointGeneration, ApiOptions apiOptions, bool useCodingRules)
>```
#### GenerateServerDomain
>```csharp
>bool GenerateServerDomain(ILogger logger, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptions apiOptions, bool useCodingRules, DirectoryInfo apiPath)
>```
#### GenerateServerHost
>```csharp
>bool GenerateServerHost(ILogger logger, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptions apiOptions, bool usingCodingRules, DirectoryInfo apiPath, DirectoryInfo domainPath)
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

## HttpClientHelper

>```csharp
>public static class HttpClientHelper
>```

### Static Methods

#### DownloadToTempFile
>```csharp
>FileInfo DownloadToTempFile(ILogger logger, string apiDesignPath)
>```
#### GetAsString
>```csharp
>string GetAsString(ILogger logger, string url, string displayName, CancellationToken cancellationToken = null)
>```

<br />

## LogItemHelper

>```csharp
>public static class LogItemHelper
>```

### Static Methods

#### Create
>```csharp
>LogKeyValueItem Create(LogCategoryType logCategoryType, string ruleName, string description)
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

## OpenApiDocumentHelper

>```csharp
>public static class OpenApiDocumentHelper
>```

### Static Methods

#### CombineAndGetApiDocument
>```csharp
>Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> CombineAndGetApiDocument(ILogger logger, string specificationPath)
>```
#### GetBasePathSegmentNames
>```csharp
>List<string> GetBasePathSegmentNames(OpenApiDocument openApiDocument)
>```
#### GetServerUrlBasePath
>```csharp
>string GetServerUrlBasePath(OpenApiDocument openApiDocument)
>```
#### Validate
>```csharp
>bool Validate(ILogger logger, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptionsValidation validationOptions)
>```

<br />

## OpenApiDocumentSchemaModelNameHelper

>```csharp
>public static class OpenApiDocumentSchemaModelNameHelper
>```

### Static Methods

#### ContainsModelNameTask
>```csharp
>bool ContainsModelNameTask(string modelName)
>```
#### EnsureModelNameWithNamespaceIfNeeded
>```csharp
>string EnsureModelNameWithNamespaceIfNeeded(EndpointMethodMetadata endpointMethodMetadata, string modelName)
>```
#### EnsureModelNameWithNamespaceIfNeeded
>```csharp
>string EnsureModelNameWithNamespaceIfNeeded(string projectName, string segmentName, string modelName, bool isShared = False, bool isClient = False)
>```
#### EnsureTaskNameWithNamespaceIfNeeded
>```csharp
>string EnsureTaskNameWithNamespaceIfNeeded(ResponseTypeNameAndItemSchema contractReturnTypeNameAndSchema)
>```
#### GetRawModelName
>```csharp
>string GetRawModelName(string modelName)
>```
#### HasList
>```csharp
>bool HasList(string typeName)
>```
#### HasSharedResponseContract
>```csharp
>bool HasSharedResponseContract(OpenApiDocument apiDocument, List<ApiOperationSchemaMap> operationSchemaMappings, string focusOnSegmentName)
>```

<br />

## OpenApiDocumentValidationHelper

>```csharp
>public static class OpenApiDocumentValidationHelper
>```

### Static Methods

#### ValidateDocument
>```csharp
>bool ValidateDocument(ILogger logger, OpenApiDocument apiDocument, ApiOptionsValidation validationOptions)
>```

<br />

## OpenApiOperationSchemaMapHelper

>```csharp
>public static class OpenApiOperationSchemaMapHelper
>```

### Static Methods

#### CollectMappings
>```csharp
>List<ApiOperationSchemaMap> CollectMappings(OpenApiDocument apiDocument)
>```
#### GetSegmentName
>```csharp
>string GetSegmentName(string path)
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

## ValidatePathsAndOperationsHelper

>```csharp
>public static class ValidatePathsAndOperationsHelper
>```

### Static Methods

#### ValidateGetOperations
>```csharp
>List<LogKeyValueItem> ValidateGetOperations(ApiOptionsValidation validationOptions, KeyValuePair<string, OpenApiPathItem> path)
>```
><b>Summary:</b> Check for response types according to operation/global parameters.
>
><b>Parameters:</b><br>
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
>
><b>Returns:</b> List of possible validation errors
#### ValidateGlobalParameters
>```csharp
>List<LogKeyValueItem> ValidateGlobalParameters(ApiOptionsValidation validationOptions, IEnumerable<string> globalPathParameterNames, KeyValuePair<string, OpenApiPathItem> path)
>```
><b>Summary:</b> Check global parameters.
>
><b>Parameters:</b><br>
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`globalPathParameterNames`&nbsp;&nbsp;-&nbsp;&nbsp;The global path parameter names.<br />
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
>
><b>Returns:</b> List of possible validation errors
#### ValidateMissingOperationParameters
>```csharp
>List<LogKeyValueItem> ValidateMissingOperationParameters(ApiOptionsValidation validationOptions, KeyValuePair<string, OpenApiPathItem> path)
>```
><b>Summary:</b> Check for operations that are not defining parameters, which are present in the path.key.
>
><b>Parameters:</b><br>
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
>
><b>Returns:</b> List of possible validation errors
#### ValidateOperationsWithParametersNotPresentInPath
>```csharp
>List<LogKeyValueItem> ValidateOperationsWithParametersNotPresentInPath(ApiOptionsValidation validationOptions, KeyValuePair<string, OpenApiPathItem> path)
>```
><b>Summary:</b> Check for operations with parameters, that are not present in the path.key.
>
><b>Parameters:</b><br>
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
>
><b>Returns:</b> List of possible validation errors
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

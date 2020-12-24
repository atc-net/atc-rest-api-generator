<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Helpers

<br />


## GenerateHelper

```csharp
public static class GenerateHelper
```

### Static Methods


#### GenerateServerApi

```csharp
List<LogKeyValueItem> GenerateServerApi(string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptions apiOptions)
```
#### GenerateServerDomain

```csharp
List<LogKeyValueItem> GenerateServerDomain(string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptions apiOptions, DirectoryInfo apiPath)
```
#### GenerateServerHost

```csharp
List<LogKeyValueItem> GenerateServerHost(string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptions apiOptions, DirectoryInfo apiPath, DirectoryInfo domainPath)
```
#### GenerateServerSln

```csharp
List<LogKeyValueItem> GenerateServerSln(string projectPrefixName, string outputSlnPath, DirectoryInfo outputSrcPath, DirectoryInfo outputTestPath)
```
#### GetAtcToolVersion

```csharp
Version GetAtcToolVersion()
```
#### GetAtcToolVersionAsString3

```csharp
string GetAtcToolVersionAsString3()
```
#### GetAtcToolVersionAsString4

```csharp
string GetAtcToolVersionAsString4()
```

<br />


## HttpClientHelper

```csharp
public static class HttpClientHelper
```

### Static Methods


#### DownloadToTempFile

```csharp
FileInfo DownloadToTempFile(string apiDesignPath)
```

<br />


## LogItemHelper

```csharp
public static class LogItemHelper
```

### Static Methods


#### Create

```csharp
LogKeyValueItem Create(LogCategoryType logCategoryType, string ruleName, string description)
```

<br />


## NugetPackageReferenceHelper

```csharp
public static class NugetPackageReferenceHelper
```

### Static Methods


#### CreateForApiProject

```csharp
List<Tuple<string, string, string>> CreateForApiProject()
```
#### CreateForHostProject

```csharp
List<Tuple<string, string, string>> CreateForHostProject(bool useRestExtended)
```
#### CreateForTestProject

```csharp
List<Tuple<string, string, string>> CreateForTestProject(bool useMvc)
```

<br />


## OpenApiDocumentHelper

```csharp
public static class OpenApiDocumentHelper
```

### Static Methods


#### CombineAndGetApiDocument

```csharp
Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> CombineAndGetApiDocument(string specificationPath)
```
#### GetBasePathSegmentNames

```csharp
List<string> GetBasePathSegmentNames(OpenApiDocument openApiDocument)
```
#### Validate

```csharp
List<LogKeyValueItem> Validate(Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptionsValidation validationOptions)
```

<br />


## OpenApiDocumentSchemaModelNameHelper

```csharp
public static class OpenApiDocumentSchemaModelNameHelper
```

### Static Methods


#### EnsureModelNameWithNamespaceIfNeeded

```csharp
string EnsureModelNameWithNamespaceIfNeeded(EndpointMethodMetadata endpointMethodMetadata, string modelName)
```
#### EnsureModelNameWithNamespaceIfNeeded

```csharp
string EnsureModelNameWithNamespaceIfNeeded(string projectName, string segmentName, string modelName, bool isShared = False)
```
#### GetRawModelName

```csharp
string GetRawModelName(string modelName)
```

<br />


## OpenApiDocumentValidationHelper

```csharp
public static class OpenApiDocumentValidationHelper
```

### Static Methods


#### ValidateDocument

```csharp
List<LogKeyValueItem> ValidateDocument(OpenApiDocument apiDocument, ApiOptionsValidation validationOptions)
```

<br />


## OpenApiOperationSchemaMapHelper

```csharp
public static class OpenApiOperationSchemaMapHelper
```

### Static Methods


#### CollectMappings

```csharp
List<ApiOperationSchemaMap> CollectMappings(OpenApiDocument apiDocument)
```
#### GetSegmentName

```csharp
string GetSegmentName(string path)
```

<br />


## SolutionAndProjectHelper

```csharp
public static class SolutionAndProjectHelper
```

### Static Methods


#### GetBoolFromNullableString

```csharp
bool GetBoolFromNullableString(string value)
```
#### GetNullableStringFromBool

```csharp
string GetNullableStringFromBool(bool value)
```
#### GetNullableValueFromProject

```csharp
string GetNullableValueFromProject(XElement element)
```
#### ScaffoldProjFile

```csharp
LogKeyValueItem ScaffoldProjFile(FileInfo projectCsProjFile, bool createAsWeb, bool createAsTestProject, string projectName, bool useNullableReferenceTypes, List<string> frameworkReferences, List<Tuple<string, string, string>> packageReferences, List<FileInfo> projectReferences, bool includeApiSpecification)
```
#### ScaffoldSlnFile

```csharp
List<LogKeyValueItem> ScaffoldSlnFile(FileInfo slnFile, string projectName, DirectoryInfo apiPath, DirectoryInfo domainPath, DirectoryInfo hostPath, DirectoryInfo apiTestPath = null, DirectoryInfo domainTestPath = null, DirectoryInfo hostTestPath = null)
```
#### SetNullableValueForProject

```csharp
void SetNullableValueForProject(XElement element, string newNullableValue)
```

<br />


## TextFileHelper

```csharp
public static class TextFileHelper
```

### Static Methods


#### Save

```csharp
LogKeyValueItem Save(string file, string text, bool overrideIfExist = True)
```
#### Save

```csharp
LogKeyValueItem Save(FileInfo fileInfo, string text, bool overrideIfExist = True)
```

<br />


## ValidatePathsAndOperationsHelper

```csharp
public static class ValidatePathsAndOperationsHelper
```

### Static Methods


#### ValidateGetOperations

```csharp
List<LogKeyValueItem> ValidateGetOperations(ApiOptionsValidation validationOptions, KeyValuePair<string, OpenApiPathItem> path)
```
<p><b>Summary:</b> Check for response types according to operation/global parameters.</p>

<b>Parameters</b>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
#### ValidateGlobalParameters

```csharp
List<LogKeyValueItem> ValidateGlobalParameters(ApiOptionsValidation validationOptions, IEnumerable<string> globalPathParameterNames, KeyValuePair<string, OpenApiPathItem> path)
```
<p><b>Summary:</b> Check global parameters.</p>

<b>Parameters</b>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`globalPathParameterNames`&nbsp;&nbsp;-&nbsp;&nbsp;The global path parameter names.<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
#### ValidateMissingOperationParameters

```csharp
List<LogKeyValueItem> ValidateMissingOperationParameters(ApiOptionsValidation validationOptions, KeyValuePair<string, OpenApiPathItem> path)
```
<p><b>Summary:</b> Check for operations that are not defining parameters, which are present in the path.key.</p>

<b>Parameters</b>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
#### ValidateOperationsWithParametersNotPresentInPath

```csharp
List<LogKeyValueItem> ValidateOperationsWithParametersNotPresentInPath(ApiOptionsValidation validationOptions, KeyValuePair<string, OpenApiPathItem> path)
```
<p><b>Summary:</b> Check for operations with parameters, that are not present in the path.key.</p>

<b>Parameters</b>

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`validationOptions`&nbsp;&nbsp;-&nbsp;&nbsp;The validation options.<br />
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`path`&nbsp;&nbsp;-&nbsp;&nbsp;The path.<br />
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

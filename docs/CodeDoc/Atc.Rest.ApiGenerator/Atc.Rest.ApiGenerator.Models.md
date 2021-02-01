<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Models

<br />


## ApiOperationSchemaMap

```csharp
public class ApiOperationSchemaMap
```

### Properties


#### LocatedArea

```csharp
LocatedArea
```
#### OperationType

```csharp
OperationType
```
#### ParentSchemaKey

```csharp
ParentSchemaKey
```
#### Path

```csharp
Path
```
#### SchemaKey

```csharp
SchemaKey
```
#### SegmentName

```csharp
SegmentName
```
### Methods


#### Equals

```csharp
bool Equals(object obj)
```
#### GetHashCode

```csharp
int GetHashCode()
```
#### ToString

```csharp
string ToString()
```

<br />


## ApiProjectOptions

```csharp
public class ApiProjectOptions : BaseProjectOptions
```

### Properties


#### PathForContracts

```csharp
PathForContracts
```
#### PathForContractsShared

```csharp
PathForContractsShared
```
#### PathForEndpoints

```csharp
PathForEndpoints
```

<br />


## BaseProjectOptions

```csharp
public abstract class BaseProjectOptions
```

### Properties


#### ApiOptions

```csharp
ApiOptions
```
#### ApiVersion

```csharp
ApiVersion
```
#### BasePathSegmentNames

```csharp
BasePathSegmentNames
```
#### ClientFolderName

```csharp
ClientFolderName
```
#### Document

```csharp
Document
```
#### DocumentFile

```csharp
DocumentFile
```
#### IsForClient

```csharp
IsForClient
```
#### PathForSrcGenerate

```csharp
PathForSrcGenerate
```
#### PathForTestGenerate

```csharp
PathForTestGenerate
```
#### ProjectName

```csharp
ProjectName
```
#### ProjectPrefixName

```csharp
ProjectPrefixName
```
#### ProjectSrcCsProj

```csharp
ProjectSrcCsProj
```
#### ProjectTestCsProj

```csharp
ProjectTestCsProj
```
#### ToolName

```csharp
ToolName
```
#### ToolNameAndVersion

```csharp
ToolNameAndVersion
```
#### ToolVersion

```csharp
ToolVersion
```

<br />


## ClientCSharpApiProjectOptions

```csharp
public class ClientCSharpApiProjectOptions
```

### Properties


#### ApiOptions

```csharp
ApiOptions
```
#### ApiVersion

```csharp
ApiVersion
```
#### BasePathSegmentNames

```csharp
BasePathSegmentNames
```
#### ClientFolderName

```csharp
ClientFolderName
```
#### Document

```csharp
Document
```
#### DocumentFile

```csharp
DocumentFile
```
#### ForClient

```csharp
ForClient
```
#### PathForSrcGenerate

```csharp
PathForSrcGenerate
```
#### ProjectName

```csharp
ProjectName
```
#### ProjectSrcCsProj

```csharp
ProjectSrcCsProj
```
#### ToolName

```csharp
ToolName
```
#### ToolNameAndVersion

```csharp
ToolNameAndVersion
```
#### ToolVersion

```csharp
ToolVersion
```

<br />


## DomainProjectOptions

```csharp
public class DomainProjectOptions : BaseProjectOptions
```

### Properties


#### ApiProjectSrcCsProj

```csharp
ApiProjectSrcCsProj
```
#### ApiProjectSrcPath

```csharp
ApiProjectSrcPath
```
#### PathForSrcHandlers

```csharp
PathForSrcHandlers
```
#### PathForTestHandlers

```csharp
PathForTestHandlers
```
### Methods


#### SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles

```csharp
List<LogKeyValueItem> SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles()
```

<br />


## EndpointMethodMetadata

```csharp
public class EndpointMethodMetadata
```

### Properties


#### ComponentsSchemas

```csharp
ComponentsSchemas
```
#### ContractInterfaceHandlerTypeName

```csharp
ContractInterfaceHandlerTypeName
```
#### ContractParameter

```csharp
ContractParameter
```
#### ContractParameterTypeName

```csharp
ContractParameterTypeName
```
#### ContractResultTypeName

```csharp
ContractResultTypeName
```
#### ContractReturnTypeNames

```csharp
ContractReturnTypeNames
```
#### HttpOperation

```csharp
HttpOperation
```
#### IsSharedResponseModel

```csharp
IsSharedResponseModel
```
#### MethodName

```csharp
MethodName
```
#### ProjectName

```csharp
ProjectName
```
#### Route

```csharp
Route
```
#### SegmentName

```csharp
SegmentName
```
#### UseNullableReferenceTypes

```csharp
UseNullableReferenceTypes
```
### Methods


#### GetHeaderParameters

```csharp
List<OpenApiParameter> GetHeaderParameters()
```
#### GetHeaderRequiredParameters

```csharp
List<OpenApiParameter> GetHeaderRequiredParameters()
```
#### GetQueryParameters

```csharp
List<OpenApiParameter> GetQueryParameters()
```
#### GetQueryRequiredParameters

```csharp
List<OpenApiParameter> GetQueryRequiredParameters()
```
#### GetRouteParameters

```csharp
List<OpenApiParameter> GetRouteParameters()
```
#### HasContractParameterRequestBody

```csharp
bool HasContractParameterRequestBody()
```
#### HasContractParameterRequiredHeader

```csharp
bool HasContractParameterRequiredHeader()
```
#### IsPaginationUsed

```csharp
bool IsPaginationUsed()
```
#### ToString

```csharp
string ToString()
```

<br />


## HostProjectOptions

```csharp
public class HostProjectOptions : BaseProjectOptions
```

### Properties


#### ApiProjectSrcCsProj

```csharp
ApiProjectSrcCsProj
```
#### ApiProjectSrcPath

```csharp
ApiProjectSrcPath
```
#### DomainProjectSrcCsProj

```csharp
DomainProjectSrcCsProj
```
#### DomainProjectSrcPath

```csharp
DomainProjectSrcPath
```
#### UseRestExtended

```csharp
UseRestExtended
```
### Methods


#### SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles

```csharp
List<LogKeyValueItem> SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles()
```

<br />


## SchemaMapLocatedAreaType

```csharp
public enum SchemaMapLocatedAreaType
```


| Value | Name | Description | Summary | 
| --- | --- | --- | --- | 
| 0 | Parameter | Parameter |  | 
| 1 | RequestBody | Request Body |  | 
| 2 | Response | Response |  | 


<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

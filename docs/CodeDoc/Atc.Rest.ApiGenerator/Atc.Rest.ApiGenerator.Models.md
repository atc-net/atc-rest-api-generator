<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Models

<br />

## ApiOperationSchemaMap

>```csharp
>public class ApiOperationSchemaMap
>```

### Properties

#### LocatedArea
>```csharp
>LocatedArea
>```
#### OperationType
>```csharp
>OperationType
>```
#### ParentSchemaKey
>```csharp
>ParentSchemaKey
>```
#### Path
>```csharp
>Path
>```
#### SchemaKey
>```csharp
>SchemaKey
>```
#### SegmentName
>```csharp
>SegmentName
>```
### Methods

#### Equals
>```csharp
>bool Equals(object obj)
>```
#### GetHashCode
>```csharp
>int GetHashCode()
>```
#### ToString
>```csharp
>string ToString()
>```

<br />

## ApiProjectOptions

>```csharp
>public class ApiProjectOptions : BaseProjectOptions
>```

### Properties

#### PathForContracts
>```csharp
>PathForContracts
>```
#### PathForContractsShared
>```csharp
>PathForContractsShared
>```
#### PathForEndpoints
>```csharp
>PathForEndpoints
>```

<br />

## BaseProjectOptions

>```csharp
>public abstract class BaseProjectOptions
>```

### Properties

#### ApiOptions
>```csharp
>ApiOptions
>```
#### BasePathSegmentNames
>```csharp
>BasePathSegmentNames
>```
#### ClientFolderName
>```csharp
>ClientFolderName
>```
#### Document
>```csharp
>Document
>```
#### DocumentFile
>```csharp
>DocumentFile
>```
#### IsForClient
>```csharp
>IsForClient
>```
#### PathForSrcGenerate
>```csharp
>PathForSrcGenerate
>```
#### PathForTestGenerate
>```csharp
>PathForTestGenerate
>```
#### ProjectName
>```csharp
>ProjectName
>```
#### ProjectPrefixName
>```csharp
>ProjectPrefixName
>```
#### ProjectSrcCsProj
>```csharp
>ProjectSrcCsProj
>```
#### ProjectTestCsProj
>```csharp
>ProjectTestCsProj
>```
#### RouteBase
>```csharp
>RouteBase
>```
#### ToolName
>```csharp
>ToolName
>```
#### ToolNameAndVersion
>```csharp
>ToolNameAndVersion
>```
#### ToolVersion
>```csharp
>ToolVersion
>```

<br />

## ClientCSharpApiProjectOptions

>```csharp
>public class ClientCSharpApiProjectOptions
>```

### Properties

#### ApiOptions
>```csharp
>ApiOptions
>```
#### BasePathSegmentNames
>```csharp
>BasePathSegmentNames
>```
#### ClientFolderName
>```csharp
>ClientFolderName
>```
#### Document
>```csharp
>Document
>```
#### DocumentFile
>```csharp
>DocumentFile
>```
#### ExcludeEndpointGeneration
>```csharp
>ExcludeEndpointGeneration
>```
#### ForClient
>```csharp
>ForClient
>```
#### PathForSrcGenerate
>```csharp
>PathForSrcGenerate
>```
#### ProjectName
>```csharp
>ProjectName
>```
#### ProjectSrcCsProj
>```csharp
>ProjectSrcCsProj
>```
#### ToolName
>```csharp
>ToolName
>```
#### ToolNameAndVersion
>```csharp
>ToolNameAndVersion
>```
#### ToolVersion
>```csharp
>ToolVersion
>```
### Methods

#### ToString
>```csharp
>string ToString()
>```

<br />

## DomainProjectOptions

>```csharp
>public class DomainProjectOptions : BaseProjectOptions
>```

### Properties

#### ApiProjectSrcCsProj
>```csharp
>ApiProjectSrcCsProj
>```
#### ApiProjectSrcPath
>```csharp
>ApiProjectSrcPath
>```
#### PathForSrcHandlers
>```csharp
>PathForSrcHandlers
>```
#### PathForTestHandlers
>```csharp
>PathForTestHandlers
>```
### Methods

#### SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles
>```csharp
>List<LogKeyValueItem> SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles()
>```

<br />

## EndpointMethodMetadata

>```csharp
>public class EndpointMethodMetadata
>```

### Properties

#### ComponentsSchemas
>```csharp
>ComponentsSchemas
>```
#### ContractInterfaceHandlerTypeName
>```csharp
>ContractInterfaceHandlerTypeName
>```
#### ContractParameter
>```csharp
>ContractParameter
>```
#### ContractParameterTypeName
>```csharp
>ContractParameterTypeName
>```
#### ContractResultTypeName
>```csharp
>ContractResultTypeName
>```
#### ContractReturnTypeNames
>```csharp
>ContractReturnTypeNames
>```
#### HttpOperation
>```csharp
>HttpOperation
>```
#### MethodName
>```csharp
>MethodName
>```
#### OperationSchemaMappings
>```csharp
>OperationSchemaMappings
>```
#### ProjectName
>```csharp
>ProjectName
>```
#### Route
>```csharp
>Route
>```
#### SegmentName
>```csharp
>SegmentName
>```
#### UseNullableReferenceTypes
>```csharp
>UseNullableReferenceTypes
>```
### Methods

#### Contains
>```csharp
>bool Contains(string value)
>```
#### GetHeaderParameters
>```csharp
>List<OpenApiParameter> GetHeaderParameters()
>```
#### GetHeaderRequiredParameters
>```csharp
>List<OpenApiParameter> GetHeaderRequiredParameters()
>```
#### GetQueryParameters
>```csharp
>List<OpenApiParameter> GetQueryParameters()
>```
#### GetQueryRequiredParameters
>```csharp
>List<OpenApiParameter> GetQueryRequiredParameters()
>```
#### GetRelevantSchemasForBadRequestBodyParameters
>```csharp
>List<KeyValuePair<string, OpenApiSchema>> GetRelevantSchemasForBadRequestBodyParameters(OpenApiSchema modelSchema)
>```
#### GetRequestBodyModelName
>```csharp
>string GetRequestBodyModelName()
>```
#### GetRequestBodySchema
>```csharp
>OpenApiSchema GetRequestBodySchema()
>```
#### GetRouteParameters
>```csharp
>List<OpenApiParameter> GetRouteParameters()
>```
#### HasContractParameterAnyParametersOrRequestBody
>```csharp
>bool HasContractParameterAnyParametersOrRequestBody()
>```
#### HasContractParameterRequestBody
>```csharp
>bool HasContractParameterRequestBody()
>```
#### HasContractReturnTypeAsComplex
>```csharp
>bool HasContractReturnTypeAsComplex()
>```
#### HasContractReturnTypeAsComplexAndNotSharedModel
>```csharp
>bool HasContractReturnTypeAsComplexAndNotSharedModel()
>```
#### HasContractReturnTypeAsComplexAsListOrPagination
>```csharp
>bool HasContractReturnTypeAsComplexAsListOrPagination()
>```
#### HasSharedModelOrEnumInContractParameterRequestBody
>```csharp
>bool HasSharedModelOrEnumInContractParameterRequestBody()
>```
#### HasSharedModelOrEnumInContractReturnType
>```csharp
>bool HasSharedModelOrEnumInContractReturnType(bool includeProperties = True)
>```
#### IsContractParameterRequestBodyUsed
>```csharp
>bool IsContractParameterRequestBodyUsed()
>```
#### IsContractParameterRequestBodyUsedAsMultipartFormData
>```csharp
>bool IsContractParameterRequestBodyUsedAsMultipartFormData()
>```
#### IsContractParameterRequestBodyUsedAsMultipartFormDataAndHasInlineSchemaFile
>```csharp
>bool IsContractParameterRequestBodyUsedAsMultipartFormDataAndHasInlineSchemaFile()
>```
#### IsContractParameterRequestBodyUsedAsMultipartOctetStreamData
>```csharp
>bool IsContractParameterRequestBodyUsedAsMultipartOctetStreamData()
>```
#### IsContractParameterRequestBodyUsingStringBuilder
>```csharp
>bool IsContractParameterRequestBodyUsingStringBuilder()
>```
#### IsContractParameterRequestBodyUsingSystemCollectionGenericNamespace
>```csharp
>bool IsContractParameterRequestBodyUsingSystemCollectionGenericNamespace()
>```
#### IsContractParameterRequestBodyUsingSystemNamespace
>```csharp
>bool IsContractParameterRequestBodyUsingSystemNamespace()
>```
#### IsContractReturnTypeUsingList
>```csharp
>bool IsContractReturnTypeUsingList()
>```
#### IsContractReturnTypeUsingPagination
>```csharp
>bool IsContractReturnTypeUsingPagination()
>```
#### IsContractReturnTypeUsingString
>```csharp
>bool IsContractReturnTypeUsingString()
>```
#### IsContractReturnTypeUsingSystemCollectionGenericNamespace
>```csharp
>bool IsContractReturnTypeUsingSystemCollectionGenericNamespace()
>```
#### IsContractReturnTypeUsingSystemNamespace
>```csharp
>bool IsContractReturnTypeUsingSystemNamespace()
>```
#### IsContractReturnTypeUsingTaskName
>```csharp
>bool IsContractReturnTypeUsingTaskName()
>```
#### IsSharedModel
>```csharp
>bool IsSharedModel(string modelName)
>```
#### ToString
>```csharp
>string ToString()
>```

<br />

## HostProjectOptions

>```csharp
>public class HostProjectOptions : BaseProjectOptions
>```

### Properties

#### ApiProjectSrcCsProj
>```csharp
>ApiProjectSrcCsProj
>```
#### ApiProjectSrcPath
>```csharp
>ApiProjectSrcPath
>```
#### DomainProjectSrcCsProj
>```csharp
>DomainProjectSrcCsProj
>```
#### DomainProjectSrcPath
>```csharp
>DomainProjectSrcPath
>```
#### UseRestExtended
>```csharp
>UseRestExtended
>```
### Methods

#### SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles
>```csharp
>List<LogKeyValueItem> SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles()
>```

<br />

## SchemaMapLocatedAreaType

>```csharp
>public enum SchemaMapLocatedAreaType
>```


| Value | Name | Description | Summary | 
| --- | --- | --- | --- | 
| 0 | Parameter | Parameter |  | 
| 1 | RequestBody | Request Body |  | 
| 2 | Response | Response |  | 


<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

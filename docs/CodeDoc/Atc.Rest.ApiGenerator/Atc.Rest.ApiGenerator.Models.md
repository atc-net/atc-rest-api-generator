<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Models

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

#### ApiGeneratorName
>```csharp
>ApiGeneratorName
>```
#### ApiGeneratorNameAndVersion
>```csharp
>ApiGeneratorNameAndVersion
>```
#### ApiGeneratorVersion
>```csharp
>ApiGeneratorVersion
>```
#### ApiGroupNames
>```csharp
>ApiGroupNames
>```
#### ApiOptions
>```csharp
>ApiOptions
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
#### ProjectSrcCsProjDisplayLocation
>```csharp
>ProjectSrcCsProjDisplayLocation
>```
#### ProjectTestCsProj
>```csharp
>ProjectTestCsProj
>```
#### ProjectTestCsProjDisplayLocation
>```csharp
>ProjectTestCsProjDisplayLocation
>```
#### RouteBase
>```csharp
>RouteBase
>```
#### UseNullableReferenceTypes
>```csharp
>UseNullableReferenceTypes
>```
#### UsingCodingRules
>```csharp
>UsingCodingRules
>```

<br />

## ClientCSharpApiProjectOptions

>```csharp
>public class ClientCSharpApiProjectOptions
>```

### Properties

#### ApiGeneratorName
>```csharp
>ApiGeneratorName
>```
#### ApiGeneratorNameAndVersion
>```csharp
>ApiGeneratorNameAndVersion
>```
#### ApiGeneratorVersion
>```csharp
>ApiGeneratorVersion
>```
#### ApiGroupNames
>```csharp
>ApiGroupNames
>```
#### ApiOptions
>```csharp
>ApiOptions
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
#### ProjectSrcCsProjDisplayLocation
>```csharp
>ProjectSrcCsProjDisplayLocation
>```
#### UseNullableReferenceTypes
>```csharp
>UseNullableReferenceTypes
>```
#### UsingCodingRules
>```csharp
>UsingCodingRules
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
>bool SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(ILogger logger)
>```

<br />

## DotnetNugetPackage

>```csharp
>public class DotnetNugetPackage
>```

### Properties

#### IsNewest
>```csharp
>IsNewest
>```
#### NewestVersion
>```csharp
>NewestVersion
>```
#### PackageId
>```csharp
>PackageId
>```
#### Version
>```csharp
>Version
>```
### Methods

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
>bool SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(ILogger logger)
>```

<br />

## ProjectType

>```csharp
>public enum ProjectType
>```


| Value | Name | Description | Summary | 
| --- | --- | --- | --- | 
| 0 | ServerHost | Server Host |  | 
| 1 | ServerApi | Server Api |  | 
| 2 | ServerDomain | Server Domain |  | 
| 3 | ClientApi | Client Api |  | 



<br />

## ResponseTypeNameAndItemSchema

>```csharp
>public class ResponseTypeNameAndItemSchema
>```

### Properties

#### FullModelName
>```csharp
>FullModelName
>```
#### HasModelName
>```csharp
>HasModelName
>```
#### HasSchema
>```csharp
>HasSchema
>```
#### Schema
>```csharp
>Schema
>```
#### StatusCode
>```csharp
>StatusCode
>```
### Methods

#### ToString
>```csharp
>string ToString()
>```
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

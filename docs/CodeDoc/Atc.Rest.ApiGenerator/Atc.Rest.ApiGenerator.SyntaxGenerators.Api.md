<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.SyntaxGenerators.Api

<br />

## SyntaxGeneratorContractInterface

>```csharp
>public class SyntaxGeneratorContractInterface : ISyntaxOperationCodeGenerator, ISyntaxCodeGenerator
>```

### Properties

#### ApiOperation
>```csharp
>ApiOperation
>```
#### ApiOperationType
>```csharp
>ApiOperationType
>```
#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### Code
>```csharp
>Code
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
#### GlobalPathParameters
>```csharp
>GlobalPathParameters
>```
#### HasParametersOrRequestBody
>```csharp
>HasParametersOrRequestBody
>```
### Methods

#### GenerateCode
>```csharp
>bool GenerateCode()
>```
#### ToCodeAsString
>```csharp
>string ToCodeAsString()
>```
#### ToFile
>```csharp
>void ToFile()
>```
#### ToFile
>```csharp
>void ToFile(FileInfo file)
>```
#### ToString
>```csharp
>string ToString()
>```

<br />

## SyntaxGeneratorContractInterfaces

>```csharp
>public class SyntaxGeneratorContractInterfaces : ISyntaxGeneratorContractInterfaces, ISyntaxGeneratorContract
>```

### Properties

#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
### Methods

#### GenerateSyntaxTrees
>```csharp
>List<SyntaxGeneratorContractInterface> GenerateSyntaxTrees()
>```

<br />

## SyntaxGeneratorContractModel

>```csharp
>public class SyntaxGeneratorContractModel : ISyntaxSchemaCodeGenerator, ISyntaxCodeGenerator
>```

### Properties

#### ApiSchema
>```csharp
>ApiSchema
>```
#### ApiSchemaKey
>```csharp
>ApiSchemaKey
>```
#### Code
>```csharp
>Code
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
#### IsEnum
>```csharp
>IsEnum
>```
#### IsForClient
>```csharp
>IsForClient
>```
#### UseOwnFolder
>```csharp
>UseOwnFolder
>```
### Methods

#### GenerateCode
>```csharp
>bool GenerateCode()
>```
#### ToCodeAsString
>```csharp
>string ToCodeAsString()
>```
#### ToFile
>```csharp
>void ToFile()
>```
#### ToFile
>```csharp
>void ToFile(FileInfo file)
>```
#### ToString
>```csharp
>string ToString()
>```

<br />

## SyntaxGeneratorContractModels

>```csharp
>public class SyntaxGeneratorContractModels : ISyntaxGeneratorContractModels, ISyntaxGeneratorContract
>```

### Properties

#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
#### OperationSchemaMappings
>```csharp
>OperationSchemaMappings
>```
### Methods

#### GenerateSyntaxTrees
>```csharp
>List<SyntaxGeneratorContractModel> GenerateSyntaxTrees()
>```

<br />

## SyntaxGeneratorContractParameter

>```csharp
>public class SyntaxGeneratorContractParameter : ISyntaxOperationCodeGenerator, ISyntaxCodeGenerator
>```

### Properties

#### ApiOperation
>```csharp
>ApiOperation
>```
#### ApiOperationType
>```csharp
>ApiOperationType
>```
#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### Code
>```csharp
>Code
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
#### GlobalPathParameters
>```csharp
>GlobalPathParameters
>```
#### IsForClient
>```csharp
>IsForClient
>```
#### UseOwnFolder
>```csharp
>UseOwnFolder
>```
### Methods

#### GenerateCode
>```csharp
>bool GenerateCode()
>```
#### ToCodeAsString
>```csharp
>string ToCodeAsString()
>```
#### ToFile
>```csharp
>void ToFile()
>```
#### ToFile
>```csharp
>void ToFile(FileInfo file)
>```
#### ToString
>```csharp
>string ToString()
>```

<br />

## SyntaxGeneratorContractParameters

>```csharp
>public class SyntaxGeneratorContractParameters : ISyntaxGeneratorContractParameters, ISyntaxGeneratorContract
>```

### Properties

#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
### Methods

#### GenerateSyntaxTrees
>```csharp
>List<SyntaxGeneratorContractParameter> GenerateSyntaxTrees()
>```

<br />

## SyntaxGeneratorContractResult

>```csharp
>public class SyntaxGeneratorContractResult : ISyntaxOperationCodeGenerator, ISyntaxCodeGenerator
>```

### Properties

#### ApiOperation
>```csharp
>ApiOperation
>```
#### ApiOperationType
>```csharp
>ApiOperationType
>```
#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### Code
>```csharp
>Code
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
#### HasCreateContentResult
>```csharp
>HasCreateContentResult
>```
### Methods

#### GenerateCode
>```csharp
>bool GenerateCode()
>```
#### ToCodeAsString
>```csharp
>string ToCodeAsString()
>```
#### ToFile
>```csharp
>void ToFile()
>```
#### ToFile
>```csharp
>void ToFile(FileInfo file)
>```
#### ToString
>```csharp
>string ToString()
>```

<br />

## SyntaxGeneratorContractResults

>```csharp
>public class SyntaxGeneratorContractResults : ISyntaxGeneratorContractResults, ISyntaxGeneratorContract
>```

### Properties

#### ApiProjectOptions
>```csharp
>ApiProjectOptions
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
### Methods

#### GenerateSyntaxTrees
>```csharp
>List<SyntaxGeneratorContractResult> GenerateSyntaxTrees()
>```

<br />

## SyntaxGeneratorEndpointControllers

>```csharp
>public class SyntaxGeneratorEndpointControllers : ISyntaxGeneratorEndpointControllers, ISyntaxCodeGenerator
>```

### Properties

#### Code
>```csharp
>Code
>```
#### FocusOnSegmentName
>```csharp
>FocusOnSegmentName
>```
### Methods

#### GenerateCode
>```csharp
>bool GenerateCode()
>```
#### GetMetadataForMethods
>```csharp
>List<EndpointMethodMetadata> GetMetadataForMethods()
>```
#### ToCodeAsString
>```csharp
>string ToCodeAsString()
>```
#### ToFile
>```csharp
>void ToFile()
>```
#### ToFile
>```csharp
>void ToFile(FileInfo file)
>```

<br />

## SyntaxGeneratorSwaggerDocOptions

>```csharp
>public class SyntaxGeneratorSwaggerDocOptions
>```

### Methods

#### GenerateCode
>```csharp
>string GenerateCode()
>```
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.SyntaxGenerators.Api

<br />


## ISyntaxGeneratorContract

```csharp
public interface ISyntaxGeneratorContract
```

### Properties


#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```

<br />


## ISyntaxGeneratorContractInterfaces

```csharp
public interface ISyntaxGeneratorContractInterfaces : ISyntaxGeneratorContract
```

### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractInterface> GenerateSyntaxTrees()
```

<br />


## ISyntaxGeneratorContractModels

```csharp
public interface ISyntaxGeneratorContractModels : ISyntaxGeneratorContract
```

### Properties


#### OperationSchemaMappings

```csharp
OperationSchemaMappings
```
### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractModel> GenerateSyntaxTrees()
```

<br />


## ISyntaxGeneratorContractParameters

```csharp
public interface ISyntaxGeneratorContractParameters : ISyntaxGeneratorContract
```

### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractParameter> GenerateSyntaxTrees()
```

<br />


## ISyntaxGeneratorContractResults

```csharp
public interface ISyntaxGeneratorContractResults : ISyntaxGeneratorContract
```

### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractResult> GenerateSyntaxTrees()
```

<br />


## ISyntaxGeneratorEndpointControllers

```csharp
public interface ISyntaxGeneratorEndpointControllers : ISyntaxCodeGenerator
```


<br />


## ISyntaxOperationCodeGenerator

```csharp
public interface ISyntaxOperationCodeGenerator : ISyntaxCodeGenerator
```

### Properties


#### ApiOperation

```csharp
ApiOperation
```
#### ApiOperationType

```csharp
ApiOperationType
```
#### ApiProjectOptions

```csharp
ApiProjectOptions
```

<br />


## SyntaxGeneratorContractInterface

```csharp
public class SyntaxGeneratorContractInterface : ISyntaxOperationCodeGenerator, ISyntaxCodeGenerator
```

### Properties


#### ApiOperation

```csharp
ApiOperation
```
#### ApiOperationType

```csharp
ApiOperationType
```
#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### Code

```csharp
Code
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
#### GlobalPathParameters

```csharp
GlobalPathParameters
```
#### HasParametersOrRequestBody

```csharp
HasParametersOrRequestBody
```
### Methods


#### GenerateCode

```csharp
bool GenerateCode()
```
#### ToCodeAsString

```csharp
string ToCodeAsString()
```
#### ToFile

```csharp
LogKeyValueItem ToFile()
```
#### ToFile

```csharp
void ToFile(FileInfo file)
```
#### ToString

```csharp
string ToString()
```

<br />


## SyntaxGeneratorContractInterfaces

```csharp
public class SyntaxGeneratorContractInterfaces : ISyntaxGeneratorContractInterfaces, ISyntaxGeneratorContract
```

### Properties


#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractInterface> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorContractModel

```csharp
public class SyntaxGeneratorContractModel : ISyntaxSchemaCodeGenerator, ISyntaxCodeGenerator
```

### Properties


#### ApiSchema

```csharp
ApiSchema
```
#### ApiSchemaKey

```csharp
ApiSchemaKey
```
#### Code

```csharp
Code
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
#### IsEnum

```csharp
IsEnum
```
### Methods


#### GenerateCode

```csharp
bool GenerateCode()
```
#### ToCodeAsString

```csharp
string ToCodeAsString()
```
#### ToFile

```csharp
LogKeyValueItem ToFile()
```
#### ToFile

```csharp
void ToFile(FileInfo file)
```
#### ToString

```csharp
string ToString()
```

<br />


## SyntaxGeneratorContractModels

```csharp
public class SyntaxGeneratorContractModels : ISyntaxGeneratorContractModels, ISyntaxGeneratorContract
```

### Properties


#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
#### OperationSchemaMappings

```csharp
OperationSchemaMappings
```
### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractModel> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorContractParameter

```csharp
public class SyntaxGeneratorContractParameter : ISyntaxOperationCodeGenerator, ISyntaxCodeGenerator
```

### Properties


#### ApiOperation

```csharp
ApiOperation
```
#### ApiOperationType

```csharp
ApiOperationType
```
#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### Code

```csharp
Code
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
#### GlobalPathParameters

```csharp
GlobalPathParameters
```
### Methods


#### GenerateCode

```csharp
bool GenerateCode()
```
#### ToCodeAsString

```csharp
string ToCodeAsString()
```
#### ToFile

```csharp
LogKeyValueItem ToFile()
```
#### ToFile

```csharp
void ToFile(FileInfo file)
```
#### ToString

```csharp
string ToString()
```

<br />


## SyntaxGeneratorContractParameters

```csharp
public class SyntaxGeneratorContractParameters : ISyntaxGeneratorContractParameters, ISyntaxGeneratorContract
```

### Properties


#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractParameter> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorContractResult

```csharp
public class SyntaxGeneratorContractResult : ISyntaxOperationCodeGenerator, ISyntaxCodeGenerator
```

### Properties


#### ApiOperation

```csharp
ApiOperation
```
#### ApiOperationType

```csharp
ApiOperationType
```
#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### Code

```csharp
Code
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
### Methods


#### GenerateCode

```csharp
bool GenerateCode()
```
#### ToCodeAsString

```csharp
string ToCodeAsString()
```
#### ToFile

```csharp
LogKeyValueItem ToFile()
```
#### ToFile

```csharp
void ToFile(FileInfo file)
```
#### ToString

```csharp
string ToString()
```

<br />


## SyntaxGeneratorContractResults

```csharp
public class SyntaxGeneratorContractResults : ISyntaxGeneratorContractResults, ISyntaxGeneratorContract
```

### Properties


#### ApiProjectOptions

```csharp
ApiProjectOptions
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
### Methods


#### GenerateSyntaxTrees

```csharp
List<SyntaxGeneratorContractResult> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorEndpointControllers

```csharp
public class SyntaxGeneratorEndpointControllers : ISyntaxGeneratorEndpointControllers, ISyntaxCodeGenerator
```

### Properties


#### Code

```csharp
Code
```
#### FocusOnSegmentName

```csharp
FocusOnSegmentName
```
### Methods


#### GenerateCode

```csharp
bool GenerateCode()
```
#### GetMetadataForMethods

```csharp
List<EndpointMethodMetadata> GetMetadataForMethods()
```
#### ToCodeAsString

```csharp
string ToCodeAsString()
```
#### ToFile

```csharp
LogKeyValueItem ToFile()
```
#### ToFile

```csharp
void ToFile(FileInfo file)
```
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

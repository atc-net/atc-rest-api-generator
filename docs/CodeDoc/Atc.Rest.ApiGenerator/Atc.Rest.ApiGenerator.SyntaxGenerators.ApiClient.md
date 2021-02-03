<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient

<br />


## SyntaxGeneratorClientEndpoint

```csharp
public class SyntaxGeneratorClientEndpoint : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
```

### Properties


#### Code

```csharp
Code
```
#### EndpointResultTypeName

```csharp
EndpointResultTypeName
```
#### EndpointTypeName

```csharp
EndpointTypeName
```
#### InterfaceTypeName

```csharp
InterfaceTypeName
```
#### ParameterTypeName

```csharp
ParameterTypeName
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


## SyntaxGeneratorClientEndpointBase

```csharp
public abstract class SyntaxGeneratorClientEndpointBase
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
#### ApiUrlPath

```csharp
ApiUrlPath
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
#### OperationSchemaMappings

```csharp
OperationSchemaMappings
```
#### ResponseTypes

```csharp
ResponseTypes
```
#### ResultTypeName

```csharp
ResultTypeName
```

<br />


## SyntaxGeneratorClientEndpointInterface

```csharp
public class SyntaxGeneratorClientEndpointInterface : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
```

### Properties


#### Code

```csharp
Code
```
#### EndpointResultTypeName

```csharp
EndpointResultTypeName
```
#### InterfaceTypeName

```csharp
InterfaceTypeName
```
#### ParameterTypeName

```csharp
ParameterTypeName
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


## SyntaxGeneratorClientEndpointInterfaces

```csharp
public class SyntaxGeneratorClientEndpointInterfaces
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
List<SyntaxGeneratorClientEndpointInterface> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorClientEndpointResult

```csharp
public class SyntaxGeneratorClientEndpointResult : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
```

### Properties


#### Code

```csharp
Code
```
#### EndpointTypeName

```csharp
EndpointTypeName
```
#### InterfaceTypeName

```csharp
InterfaceTypeName
```
#### ParameterTypeName

```csharp
ParameterTypeName
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


## SyntaxGeneratorClientEndpointResultInterface

```csharp
public class SyntaxGeneratorClientEndpointResultInterface : SyntaxGeneratorClientEndpointBase, ISyntaxCodeGenerator
```

### Properties


#### Code

```csharp
Code
```
#### InterfaceTypeName

```csharp
InterfaceTypeName
```
#### ParameterTypeName

```csharp
ParameterTypeName
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


## SyntaxGeneratorClientEndpointResultInterfaces

```csharp
public class SyntaxGeneratorClientEndpointResultInterfaces
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
List<SyntaxGeneratorClientEndpointResultInterface> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorClientEndpointResults

```csharp
public class SyntaxGeneratorClientEndpointResults
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
List<SyntaxGeneratorClientEndpointResult> GenerateSyntaxTrees()
```

<br />


## SyntaxGeneratorClientEndpoints

```csharp
public class SyntaxGeneratorClientEndpoints : ISyntaxGeneratorClientEndpoints
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
List<SyntaxGeneratorClientEndpoint> GenerateSyntaxTrees()
```
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

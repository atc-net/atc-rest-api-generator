<div style='text-align: right'>

[References](Index.md)&nbsp;&nbsp;-&nbsp;&nbsp;[References extended](IndexExtended.md)
</div>

# Atc.Rest.ApiGenerator.Helpers.XunitTest

<br />


## GenerateServerApiXunitTestEndpointHandlerStubHelper

```csharp
public static class GenerateServerApiXunitTestEndpointHandlerStubHelper
```

### Static Methods


#### Generate

```csharp
LogKeyValueItem Generate(HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata)
```

<br />


## GenerateServerApiXunitTestEndpointTestHelper

```csharp
public static class GenerateServerApiXunitTestEndpointTestHelper
```

### Static Methods


#### Generate

```csharp
LogKeyValueItem Generate(HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata)
```

<br />


## GenerateServerDomainXunitTestHelper

```csharp
public static class GenerateServerDomainXunitTestHelper
```

### Static Methods


#### GenerateCustomTests

```csharp
LogKeyValueItem GenerateCustomTests(DomainProjectOptions domainProjectOptions, SyntaxGeneratorHandler sgHandler)
```
#### GenerateGeneratedTests

```csharp
LogKeyValueItem GenerateGeneratedTests(DomainProjectOptions domainProjectOptions, SyntaxGeneratorHandler sgHandler)
```

<br />


## GenerateXunitTestHelper

```csharp
public static class GenerateXunitTestHelper
```

### Static Methods


#### AppendDataEqualNewListOfModel

```csharp
void AppendDataEqualNewListOfModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair<string, OpenApiSchema> schemaProperty, TrailingCharType trailingChar, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair<KeyValuePair<string, OpenApiSchema>> badPropertySchema, bool asJsonBody)
```
#### AppendModel

```csharp
void AppendModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, string modelName, OpenApiSchema schema, TrailingCharType trailingChar, int itemNumber, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair<KeyValuePair<string, OpenApiSchema>> badPropertySchema, bool asJsonBody, string parentModelNameToJsonBody = null)
```
#### AppendModelComplexProperty

```csharp
void AppendModelComplexProperty(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair<string, OpenApiSchema> schemaProperty, string dataType, TrailingCharType trailingChar, int itemNumber, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair<KeyValuePair<string, OpenApiSchema>> badPropertySchema, bool asJsonBody)
```
#### AppendVarDataEqualNewListOfModel

```csharp
void AppendVarDataEqualNewListOfModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair<string, OpenApiSchema> schemaProperty, TrailingCharType trailingChar, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair<KeyValuePair<string, OpenApiSchema>> badPropertySchema, bool asJsonBody)
```
#### AppendVarDataModelOrListOfModel

```csharp
void AppendVarDataModelOrListOfModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, OpenApiSchema schema, HttpStatusCode httpStatusCode, SchemaMapLocatedAreaType locatedAreaType, KeyValuePair<KeyValuePair<string, OpenApiSchema>> badPropertySchema = null, bool asJsonBody = False, int maxItemsForList = 3, int depthHierarchy = 0, int maxDepthHierarchy = 2)
```

<br />


## GenerateXunitTestPartsHelper

```csharp
public static class GenerateXunitTestPartsHelper
```

### Static Methods


#### AppendModelSimpleProperty

```csharp
int AppendModelSimpleProperty(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair<string, OpenApiSchema> schemaProperty, string dataType, bool isRequired, string propertyValueGenerated, int countString, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendModelSimplePropertyDefault

```csharp
void AppendModelSimplePropertyDefault(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendModelSimplePropertyForDateTimeOffset

```csharp
void AppendModelSimplePropertyForDateTimeOffset(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendModelSimplePropertyForGuid

```csharp
void AppendModelSimplePropertyForGuid(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendModelSimplePropertyForIFormFile

```csharp
void AppendModelSimplePropertyForIFormFile(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendModelSimplePropertyForString

```csharp
int AppendModelSimplePropertyForString(int indentSpaces, StringBuilder sb, string propertyName, KeyValuePair<string, OpenApiSchema> schemaProperty, string propertyValueGenerated, int countString, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendModelSimplePropertyForUri

```csharp
void AppendModelSimplePropertyForUri(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
```
#### AppendPartDataEqualNew

```csharp
void AppendPartDataEqualNew(int indentSpaces, StringBuilder sb, string variableName)
```
#### AppendPartDataEqualNewListOf

```csharp
void AppendPartDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
```
#### AppendPartDataNew

```csharp
void AppendPartDataNew(int indentSpaces, StringBuilder sb)
```
#### AppendPartVarDataEqualNew

```csharp
void AppendPartVarDataEqualNew(int indentSpaces, StringBuilder sb, string variableName = data)
```
#### AppendPartVarDataEqualNewListOf

```csharp
void AppendPartVarDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
```
#### GetDataTypeIfEnum

```csharp
string GetDataTypeIfEnum(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas)
```
#### GetTrailingCharForProperty

```csharp
TrailingCharType GetTrailingCharForProperty(bool asJsonBody, int currentItem, int totalItems)
```
#### GetTrailingCharForProperty

```csharp
TrailingCharType GetTrailingCharForProperty(bool asJsonBody, KeyValuePair<string, OpenApiSchema> currentSchema, IDictionary<string, OpenApiSchema> totalSchema)
```
#### IsListKind

```csharp
bool IsListKind(string typeName)
```
#### PropertyValueGenerator

```csharp
string PropertyValueGenerator(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest, int itemNumber, string customValue)
```
#### PropertyValueGeneratorTypeResolver

```csharp
string PropertyValueGeneratorTypeResolver(KeyValuePair<string, OpenApiSchema> schema, IDictionary<string, OpenApiSchema> componentsSchemas, bool useForBadRequest)
```
#### WrapInQuotes

```csharp
string WrapInQuotes(string str)
```
#### WrapInStringBuilderAppendLine

```csharp
string WrapInStringBuilderAppendLine(string str)
```
#### WrapInStringBuilderAppendLineWithKeyAndValueQuotes

```csharp
string WrapInStringBuilderAppendLineWithKeyAndValueQuotes(int depthHierarchy, string key, string value, TrailingCharType trailingChar)
```
#### WrapInStringBuilderAppendLineWithKeyQuotes

```csharp
string WrapInStringBuilderAppendLineWithKeyQuotes(int depthHierarchy, string key, string value, TrailingCharType trailingChar)
```

<br />


## ParameterCombinationHelper

```csharp
public static class ParameterCombinationHelper
```

### Static Methods


#### GetCombination

```csharp
List<List<OpenApiParameter>> GetCombination(List<OpenApiParameter> parameters, bool useForBadRequest)
```

<br />


## TrailingCharType

```csharp
public enum TrailingCharType
```


| Value | Name | Description | Summary | 
| --- | --- | --- | --- | 
| 0 | None | None |  | 
| 1 | Comma | Comma |  | 
| 2 | SemiColon | Semi Colon |  | 
| 3 | Colon | Colon |  | 



<br />


## ValueTypeTestPropertiesHelper

```csharp
public static class ValueTypeTestPropertiesHelper
```

### Static Methods


#### CreateValueArray

```csharp
string CreateValueArray(string name, OpenApiSchema itemSchema, ParameterLocation? parameterLocation, bool useForBadRequest, int count)
```
#### CreateValueBool

```csharp
string CreateValueBool(bool useForBadRequest)
```
#### CreateValueDateTimeOffset

```csharp
string CreateValueDateTimeOffset(bool useForBadRequest)
```
#### CreateValueEmail

```csharp
string CreateValueEmail(bool useForBadRequest, int itemNumber = 0)
```
#### CreateValueEnum

```csharp
string CreateValueEnum(string name, KeyValuePair<string, OpenApiSchema> schemaForEnum, bool useForBadRequest)
```
#### CreateValueGuid

```csharp
string CreateValueGuid(bool useForBadRequest, int itemNumber = 0)
```
#### CreateValueString

```csharp
string CreateValueString(string name, OpenApiSchema schema, ParameterLocation? parameterLocation, bool useForBadRequest, int itemNumber = 0, string customValue = null)
```
#### CreateValueUri

```csharp
string CreateValueUri(bool useForBadRequest)
```
#### Number

```csharp
string Number(string name, OpenApiSchema schema, bool useForBadRequest)
```
<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

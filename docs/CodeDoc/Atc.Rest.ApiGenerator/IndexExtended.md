<div style='text-align: right'>
[References](Index.md)
</div>

# References extended

## [Atc.Rest.ApiGenerator](Atc.Rest.ApiGenerator.md)

- [AppEmojisConstants](Atc.Rest.ApiGenerator.md#appemojisconstants)
  -  Static Fields
     - string AreaCodingRules
     - string AreaDirectoryBuildProps
     - string AreaDownload
     - string AreaEditorConfig
     - string AreaGenerateCode
     - string AreaGenerateTest
     - string PackageReference
- [AtcRestApiGeneratorAssemblyTypeInitializer](Atc.Rest.ApiGenerator.md#atcrestapigeneratorassemblytypeinitializer)
- [Constants](Atc.Rest.ApiGenerator.md#constants)
  -  Static Fields
     - string GitHubPrefix
     - string GitRawContentUrl

## [Atc.Rest.ApiGenerator.Extensions](Atc.Rest.ApiGenerator.Extensions.md)

- [ListExtensions](Atc.Rest.ApiGenerator.Extensions.md#listextensions)
  -  Static Methods
     - TrimEndForEmptyValues(this IList&lt;string&gt; values)

## [Atc.Rest.ApiGenerator.Generators](Atc.Rest.ApiGenerator.Generators.md)

- [ClientCSharpApiGenerator](Atc.Rest.ApiGenerator.Generators.md#clientcsharpapigenerator)
  -  Methods
     - Generate()
- [ServerApiGenerator](Atc.Rest.ApiGenerator.Generators.md#serverapigenerator)
  -  Methods
     - Generate()
- [ServerDomainGenerator](Atc.Rest.ApiGenerator.Generators.md#serverdomaingenerator)
  -  Methods
     - Generate()
- [ServerHostGenerator](Atc.Rest.ApiGenerator.Generators.md#serverhostgenerator)
  -  Methods
     - Generate()

## [Atc.Rest.ApiGenerator.Helpers](Atc.Rest.ApiGenerator.Helpers.md)

- [AtcApiNugetClientHelper](Atc.Rest.ApiGenerator.Helpers.md#atcapinugetclienthelper)
  -  Static Methods
     - GetLatestVersionForPackageId(ILogger logger, string packageId, CancellationToken cancellationToken = null)
     - GetLatestVersionForPackageId(string packageId, CancellationToken cancellationToken = null)
- [DirectoryInfoHelper](Atc.Rest.ApiGenerator.Helpers.md#directoryinfohelper)
  -  Static Methods
     - GetCsFileNameForContract(DirectoryInfo pathForContracts, string area, string modelName)
     - GetCsFileNameForContract(DirectoryInfo pathForContracts, string area, string subArea, string modelName)
     - GetCsFileNameForContractEnumTypes(DirectoryInfo pathForContracts, string modelName)
     - GetCsFileNameForContractShared(DirectoryInfo pathForContracts, string modelName)
     - GetCsFileNameForEndpoints(DirectoryInfo pathForEndpoints, string modelName)
     - GetCsFileNameForHandler(DirectoryInfo pathForHandlers, string area, string handlerName)
     - GetProjectPath()
- [GenerateAtcCodingRulesHelper](Atc.Rest.ApiGenerator.Helpers.md#generateatccodingruleshelper)
  -  Static Fields
     - string FileNameDirectoryBuildProps
     - string FileNameEditorConfig
  -  Static Methods
     - Generate(ILogger logger, string outputSlnPath, DirectoryInfo outputSrcPath, DirectoryInfo outputTestPath)
- [GenerateHelper](Atc.Rest.ApiGenerator.Helpers.md#generatehelper)
  -  Static Methods
     - GenerateServerApi(ILogger logger, IApiOperationExtractor apiOperationExtractor, string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool useCodingRules)
     - GenerateServerCSharpClient(ILogger logger, IApiOperationExtractor apiOperationExtractor, string projectPrefixName, string clientFolderName, DirectoryInfo outputPath, OpenApiDocumentContainer apiDocumentContainer, bool excludeEndpointGeneration, ApiOptions apiOptions, bool useCodingRules)
     - GenerateServerDomain(ILogger logger, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool useCodingRules, DirectoryInfo apiPath)
     - GenerateServerHost(ILogger logger, IApiOperationExtractor apiOperationExtractor, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool usingCodingRules, DirectoryInfo apiPath, DirectoryInfo domainPath)
     - GenerateServerSln(ILogger logger, string projectPrefixName, string outputSlnPath, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath)
     - GetAtcApiGeneratorVersion()
     - GetAtcApiGeneratorVersionAsString3()
     - GetAtcApiGeneratorVersionAsString4()
     - GetAtcVersion()
     - GetAtcVersionAsString3()
     - GetAtcVersionAsString4()
- [GlobalUsingsHelper](Atc.Rest.ApiGenerator.Helpers.md#globalusingshelper)
  -  Static Methods
     - CreateOrUpdate(ILogger logger, ContentWriterArea contentWriterArea, DirectoryInfo directoryInfo, List&lt;string&gt; requiredUsings)
- [NugetPackageReferenceHelper](Atc.Rest.ApiGenerator.Helpers.md#nugetpackagereferencehelper)
  -  Static Methods
     - CreateForApiProject()
     - CreateForClientApiProject()
     - CreateForHostProject(bool useRestExtended)
     - CreateForTestProject(bool useMvc)
- [OpenApiDocumentSchemaModelNameHelper](Atc.Rest.ApiGenerator.Helpers.md#openapidocumentschemamodelnamehelper)
  -  Static Methods
     - ContainsModelNameTask(string modelName)
     - EnsureModelNameWithNamespaceIfNeeded(EndpointMethodMetadata endpointMethodMetadata, string modelName)
     - EnsureTaskNameWithNamespaceIfNeeded(ResponseTypeNameAndItemSchema contractReturnTypeNameAndSchema)
- [SolutionAndProjectHelper](Atc.Rest.ApiGenerator.Helpers.md#solutionandprojecthelper)
  -  Static Methods
     - EnsureLatestPackageReferencesVersionInProjFile(ILogger logger, FileInfo projectCsProjFile, string fileDisplayLocation, ProjectType projectType, bool isTestProject)
     - ScaffoldProjFile(ILogger logger, FileInfo projectCsProjFile, string fileDisplayLocation, ProjectType projectType, bool createAsWeb, bool createAsTestProject, string projectName, string targetFramework, IList&lt;string&gt; frameworkReferences, IList&lt;Tuple&lt;string, string, string&gt;&gt; packageReferences, IList&lt;FileInfo&gt; projectReferences, bool includeApiSpecification, bool usingCodingRules)
     - ScaffoldSlnFile(ILogger logger, FileInfo slnFile, string projectName, DirectoryInfo apiPath, DirectoryInfo domainPath, DirectoryInfo hostPath, DirectoryInfo domainTestPath = null, DirectoryInfo hostTestPath = null)

## [Atc.Rest.ApiGenerator.Helpers.XunitTest](Atc.Rest.ApiGenerator.Helpers.XunitTest.md)

- [GenerateServerApiXunitTestEndpointHandlerStubHelper](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#generateserverapixunittestendpointhandlerstubhelper)
  -  Static Methods
     - Generate(ILogger logger, HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata, string apiGroupName, ContentGeneratorServerControllerMethodParameters methodParameters)
- [GenerateServerApiXunitTestEndpointTestHelper](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#generateserverapixunittestendpointtesthelper)
  -  Static Methods
     - Generate(ILogger logger, HostProjectOptions hostProjectOptions, EndpointMethodMetadata endpointMethodMetadata)
- [GenerateXunitTestHelper](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#generatexunittesthelper)
  -  Static Methods
     - AppendDataEqualNewListOfModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair&lt;string, OpenApiSchema&gt; schemaProperty, TrailingCharType trailingChar, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair&lt;KeyValuePair&lt;string, OpenApiSchema&gt;&gt; badPropertySchema, bool asJsonBody)
     - AppendModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, string modelName, OpenApiSchema schema, TrailingCharType trailingChar, int itemNumber, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair&lt;KeyValuePair&lt;string, OpenApiSchema&gt;&gt; badPropertySchema, bool asJsonBody, string parentModelNameToJsonBody = null)
     - AppendModelComplexProperty(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair&lt;string, OpenApiSchema&gt; schemaProperty, string dataType, TrailingCharType trailingChar, int itemNumber, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair&lt;KeyValuePair&lt;string, OpenApiSchema&gt;&gt; badPropertySchema, bool asJsonBody)
     - AppendVarDataEqualNewListOfModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair&lt;string, OpenApiSchema&gt; schemaProperty, TrailingCharType trailingChar, int maxItemsForList, int depthHierarchy, int maxDepthHierarchy, KeyValuePair&lt;KeyValuePair&lt;string, OpenApiSchema&gt;&gt; badPropertySchema, bool asJsonBody)
     - AppendVarDataListSimpleType(int indentSpaces, StringBuilder sb, string simpleDataType, int maxItemsForList = 3)
     - AppendVarDataModelOrListOfModel(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, OpenApiSchema schema, HttpStatusCode httpStatusCode, ApiSchemaMapLocatedAreaType locatedAreaType, KeyValuePair&lt;KeyValuePair&lt;string, OpenApiSchema&gt;&gt; badPropertySchema = null, bool asJsonBody = False, int maxItemsForList = 3, int depthHierarchy = 0, int maxDepthHierarchy = 2)
- [GenerateXunitTestPartsHelper](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#generatexunittestpartshelper)
  -  Static Methods
     - AppendModelSimpleProperty(int indentSpaces, StringBuilder sb, EndpointMethodMetadata endpointMethodMetadata, KeyValuePair&lt;string, OpenApiSchema&gt; schemaProperty, string dataType, bool isRequired, string propertyValueGenerated, int countString, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendModelSimplePropertyDefault(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendModelSimplePropertyForDateTimeOffset(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendModelSimplePropertyForGuid(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendModelSimplePropertyForIFormFile(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendModelSimplePropertyForString(int indentSpaces, StringBuilder sb, string propertyName, KeyValuePair&lt;string, OpenApiSchema&gt; schemaProperty, string propertyValueGenerated, int countString, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendModelSimplePropertyForUri(int indentSpaces, StringBuilder sb, string propertyName, string propertyValueGenerated, bool asJsonBody, int depthHierarchy, TrailingCharType trailingChar)
     - AppendPartDataEqualNew(int indentSpaces, StringBuilder sb, string variableName)
     - AppendPartDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
     - AppendPartDataNew(int indentSpaces, StringBuilder sb)
     - AppendPartVarDataEqualNew(int indentSpaces, StringBuilder sb, string variableName = data)
     - AppendPartVarDataEqualNewListOf(int indentSpaces, StringBuilder sb, string variableName, string dataType)
     - GetDataTypeIfEnum(KeyValuePair&lt;string, OpenApiSchema&gt; schema, IDictionary&lt;string, OpenApiSchema&gt; componentsSchemas)
     - GetTrailingCharForProperty(bool asJsonBody, int currentItem, int totalItems)
     - GetTrailingCharForProperty(bool asJsonBody, KeyValuePair&lt;string, OpenApiSchema&gt; currentSchema, IDictionary&lt;string, OpenApiSchema&gt; totalSchema)
     - IsListKind(string typeName)
     - PropertyValueGenerator(KeyValuePair&lt;string, OpenApiSchema&gt; schema, IDictionary&lt;string, OpenApiSchema&gt; componentsSchemas, bool useForBadRequest, int itemNumber, string customValue)
     - PropertyValueGeneratorTypeResolver(KeyValuePair&lt;string, OpenApiSchema&gt; schema, IDictionary&lt;string, OpenApiSchema&gt; componentsSchemas, bool useForBadRequest)
     - WrapInQuotes(string str)
     - WrapInStringBuilderAppendLine(string str)
     - WrapInStringBuilderAppendLineWithKeyAndValueQuotes(int depthHierarchy, string key, string value, TrailingCharType trailingChar)
     - WrapInStringBuilderAppendLineWithKeyQuotes(int depthHierarchy, string key, string value, TrailingCharType trailingChar)
- [ParameterCombinationHelper](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#parametercombinationhelper)
  -  Static Methods
     - GetCombination(List&lt;OpenApiParameter&gt; parameters, bool useForBadRequest)
- [TrailingCharType](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#trailingchartype)
- [ValueTypeTestPropertiesHelper](Atc.Rest.ApiGenerator.Helpers.XunitTest.md#valuetypetestpropertieshelper)
  -  Static Methods
     - CreateValueArray(string name, OpenApiSchema itemSchema, ParameterLocation? parameterLocation, bool useForBadRequest, int count)
     - CreateValueBool(bool useForBadRequest)
     - CreateValueDateTimeOffset(bool useForBadRequest, int itemNumber = 0)
     - CreateValueEmail(bool useForBadRequest, int itemNumber = 0)
     - CreateValueEnum(string name, KeyValuePair&lt;string, OpenApiSchema&gt; schemaForEnum, bool useForBadRequest)
     - CreateValueGuid(bool useForBadRequest, int itemNumber = 0)
     - CreateValueString(string name, OpenApiSchema schema, ParameterLocation? parameterLocation, bool useForBadRequest, int itemNumber = 0, string customValue = null)
     - CreateValueUri(bool useForBadRequest)
     - Number(string name, OpenApiSchema schema, bool useForBadRequest)

## [Atc.Rest.ApiGenerator.Models](Atc.Rest.ApiGenerator.Models.md)

- [ApiProjectOptions](Atc.Rest.ApiGenerator.Models.md#apiprojectoptions)
  -  Properties
     - PathForContracts
     - PathForContractsShared
     - PathForEndpoints
- [BaseProjectOptions](Atc.Rest.ApiGenerator.Models.md#baseprojectoptions)
  -  Properties
     - ApiGeneratorName
     - ApiGeneratorNameAndVersion
     - ApiGeneratorVersion
     - ApiGroupNames
     - ApiOptions
     - ClientFolderName
     - Document
     - DocumentFile
     - IsForClient
     - PathForSrcGenerate
     - PathForTestGenerate
     - ProjectName
     - ProjectPrefixName
     - ProjectSrcCsProj
     - ProjectSrcCsProjDisplayLocation
     - ProjectTestCsProj
     - ProjectTestCsProjDisplayLocation
     - RouteBase
     - UseNullableReferenceTypes
     - UsingCodingRules
- [ClientCSharpApiProjectOptions](Atc.Rest.ApiGenerator.Models.md#clientcsharpapiprojectoptions)
  -  Properties
     - ApiGeneratorName
     - ApiGeneratorNameAndVersion
     - ApiGeneratorVersion
     - ApiGroupNames
     - ApiOptions
     - ClientFolderName
     - Document
     - DocumentFile
     - ExcludeEndpointGeneration
     - ForClient
     - PathForSrcGenerate
     - ProjectName
     - ProjectSrcCsProj
     - ProjectSrcCsProjDisplayLocation
     - UseNullableReferenceTypes
     - UsingCodingRules
  -  Methods
     - ToString()
- [DomainProjectOptions](Atc.Rest.ApiGenerator.Models.md#domainprojectoptions)
  -  Properties
     - ApiProjectSrcCsProj
     - ApiProjectSrcPath
     - PathForSrcHandlers
     - PathForTestHandlers
  -  Methods
     - SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(ILogger logger)
- [DotnetNugetPackage](Atc.Rest.ApiGenerator.Models.md#dotnetnugetpackage)
  -  Properties
     - IsNewest
     - NewestVersion
     - PackageId
     - Version
  -  Methods
     - ToString()
- [EndpointMethodMetadata](Atc.Rest.ApiGenerator.Models.md#endpointmethodmetadata)
  -  Properties
     - ApiGroupName
     - ApiOperation
     - ComponentsSchemas
     - ContractInterfaceHandlerTypeName
     - ContractParameterTypeName
     - ContractResultTypeName
     - ContractReturnTypeNames
     - GlobalPathParameters
     - HttpOperation
     - MethodName
     - OperationSchemaMappings
     - ProjectName
     - Route
     - UseNullableReferenceTypes
  -  Methods
     - Contains(string value)
     - GetHeaderParameters()
     - GetHeaderRequiredParameters()
     - GetQueryParameters()
     - GetQueryRequiredParameters()
     - GetRelevantSchemasForBadRequestBodyParameters(OpenApiSchema modelSchema)
     - GetRequestBodyModelName()
     - GetRequestBodySchema()
     - GetRouteParameters()
     - HasContractParameterAnyParametersOrRequestBody()
     - HasContractParameterRequestBody()
     - HasContractReturnTypeAsComplex()
     - HasContractReturnTypeAsComplexAsListOrPagination()
     - HasSharedModelOrEnumInContractParameterRequestBody()
     - HasSharedModelOrEnumInContractReturnType(bool includeProperties = True)
     - IsContractParameterRequestBodyUsed()
     - IsContractParameterRequestBodyUsedAsMultipartFormData()
     - IsContractParameterRequestBodyUsedAsMultipartFormDataAndHasInlineSchemaFile()
     - IsContractParameterRequestBodyUsedAsMultipartOctetStreamData()
     - IsContractParameterRequestBodyUsingStringBuilder()
     - IsContractParameterRequestBodyUsingSystemCollectionGenericNamespace()
     - IsContractParameterRequestBodyUsingSystemNamespace()
     - IsContractReturnTypeUsingList()
     - IsContractReturnTypeUsingPagination()
     - IsContractReturnTypeUsingTaskName()
     - IsSharedModel(string modelName)
     - ToString()
- [HostProjectOptions](Atc.Rest.ApiGenerator.Models.md#hostprojectoptions)
  -  Properties
     - ApiProjectSrcCsProj
     - ApiProjectSrcPath
     - DomainProjectSrcCsProj
     - DomainProjectSrcPath
     - UseRestExtended
  -  Methods
     - SetPropertiesAfterValidationsOfProjectReferencesPathAndFiles(ILogger logger)
- [ProjectType](Atc.Rest.ApiGenerator.Models.md#projecttype)

## [Atc.Rest.ApiGenerator.SyntaxGenerators](Atc.Rest.ApiGenerator.SyntaxGenerators.md)

- [ResponseTypeNameAndItemSchema](Atc.Rest.ApiGenerator.SyntaxGenerators.md#responsetypenameanditemschema)
  -  Properties
     - FullModelName
     - HasModelName
     - HasSchema
     - Schema
     - StatusCode
  -  Methods
     - ToString()

## [Atc.Rest.ApiGenerator.SyntaxGenerators.Api](Atc.Rest.ApiGenerator.SyntaxGenerators.Api.md)

- [SyntaxGeneratorContractParameter](Atc.Rest.ApiGenerator.SyntaxGenerators.Api.md#syntaxgeneratorcontractparameter)
  -  Properties
     - ApiOperation
     - GlobalPathParameters
- [SyntaxGeneratorContractParameters](Atc.Rest.ApiGenerator.SyntaxGenerators.Api.md#syntaxgeneratorcontractparameters)
  -  Properties
     - ApiGroupName
     - ApiProjectOptions
  -  Methods
     - GenerateSyntaxTrees()
- [SyntaxGeneratorEndpointControllers](Atc.Rest.ApiGenerator.SyntaxGenerators.Api.md#syntaxgeneratorendpointcontrollers)
  -  Properties
     - ApiGroupName
     - Code
  -  Methods
     - GenerateCode()
     - GetMetadataForMethods()

## [Microsoft.OpenApi.Models](Microsoft.OpenApi.Models.md)

- [OpenApiSchemaExtensions](Microsoft.OpenApi.Models.md#openapischemaextensions)
  -  Static Methods
     - HasAnySharedModel(this OpenApiSchema schema, List&lt;ApiOperation&gt; apiOperationSchemaMaps)
     - HasAnySharedModelOrEnum(this OpenApiSchema schema, IList&lt;ApiOperation&gt; apiOperationSchemaMaps, bool includeProperties = True)

<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

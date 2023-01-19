<div style='text-align: right'>
[References](Index.md)
</div>

# References extended

## [Atc.Rest.ApiGenerator](Atc.Rest.ApiGenerator.md)

- [AppEmojisConstants](Atc.Rest.ApiGenerator.md#appemojisconstants)
  -  Static Fields
     - string AreaGenerateCode
     - string AreaGenerateTest
     - string PackageReference
- [AtcRestApiGeneratorAssemblyTypeInitializer](Atc.Rest.ApiGenerator.md#atcrestapigeneratorassemblytypeinitializer)

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

- [DirectoryInfoHelper](Atc.Rest.ApiGenerator.Helpers.md#directoryinfohelper)
  -  Static Methods
     - GetCsFileNameForContract(DirectoryInfo pathForContracts, string apiGroupName, string modelName)
     - GetCsFileNameForContract(DirectoryInfo pathForContracts, string apiGroupName, string subArea, string modelName)
     - GetCsFileNameForContractEnumTypes(DirectoryInfo pathForContracts, string modelName)
     - GetCsFileNameForContractShared(DirectoryInfo pathForContracts, string modelName)
     - GetCsFileNameForEndpoints(DirectoryInfo pathForEndpoints, string modelName)
     - GetCsFileNameForHandler(DirectoryInfo pathForHandlers, string apiGroupName, string handlerName)
     - GetProjectPath()
- [GenerateHelper](Atc.Rest.ApiGenerator.Helpers.md#generatehelper)
  -  Static Methods
     - GenerateServerApi(ILogger logger, IApiOperationExtractor apiOperationExtractor, INugetPackageReferenceProvider nugetPackageReferenceProvider, string projectPrefixName, DirectoryInfo outputPath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool useCodingRules)
     - GenerateServerCSharpClient(ILogger logger, IApiOperationExtractor apiOperationExtractor, INugetPackageReferenceProvider nugetPackageReferenceProvider, string projectPrefixName, string clientFolderName, DirectoryInfo outputPath, OpenApiDocumentContainer apiDocumentContainer, bool excludeEndpointGeneration, ApiOptions apiOptions, bool useCodingRules)
     - GenerateServerDomain(ILogger logger, INugetPackageReferenceProvider nugetPackageReferenceProvider, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool useCodingRules, DirectoryInfo apiPath)
     - GenerateServerHost(ILogger logger, INugetPackageReferenceProvider nugetPackageReferenceProvider, string projectPrefixName, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath, OpenApiDocumentContainer apiDocumentContainer, ApiOptions apiOptions, bool usingCodingRules, DirectoryInfo apiPath, DirectoryInfo domainPath)
     - GenerateServerSln(ILogger logger, string projectPrefixName, string outputSlnPath, DirectoryInfo outputSourcePath, DirectoryInfo outputTestPath)
- [GlobalUsingsHelper](Atc.Rest.ApiGenerator.Helpers.md#globalusingshelper)
  -  Static Methods
     - CreateOrUpdate(ILogger logger, ContentWriterArea contentWriterArea, DirectoryInfo directoryInfo, List&lt;string&gt; requiredUsings)
- [SolutionAndProjectHelper](Atc.Rest.ApiGenerator.Helpers.md#solutionandprojecthelper)
  -  Static Methods
     - EnsureLatestPackageReferencesVersionInProjFile(ILogger logger, FileInfo projectCsProjFile, string fileDisplayLocation, ProjectType projectType, bool isTestProject)
     - ScaffoldProjFile(ILogger logger, FileInfo projectCsProjFile, string fileDisplayLocation, ProjectType projectType, bool createAsWeb, bool createAsTestProject, string projectName, string targetFramework, IList&lt;string&gt; frameworkReferences, IList&lt;ValueTuple&lt;string, string, string&gt;&gt; packageReferences, IList&lt;FileInfo&gt; projectReferences, bool includeApiSpecification, bool usingCodingRules)
     - ScaffoldSlnFile(ILogger logger, FileInfo slnFile, string projectName, DirectoryInfo apiPath, DirectoryInfo domainPath, DirectoryInfo hostPath, DirectoryInfo domainTestPath = null, DirectoryInfo hostTestPath = null)
- [TrailingCharType](Atc.Rest.ApiGenerator.Helpers.md#trailingchartype)

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
- [ResponseTypeNameAndItemSchema](Atc.Rest.ApiGenerator.Models.md#responsetypenameanditemschema)
  -  Properties
     - FullModelName
     - HasModelName
     - HasSchema
     - Schema
     - StatusCode
  -  Methods
     - ToString()

## [Microsoft.OpenApi.Models](Microsoft.OpenApi.Models.md)

- [OpenApiSchemaExtensions](Microsoft.OpenApi.Models.md#openapischemaextensions)
  -  Static Methods
     - HasAnySharedModel(this OpenApiSchema schema, List&lt;ApiOperation&gt; apiOperationSchemaMaps)
     - HasAnySharedModelOrEnum(this OpenApiSchema schema, IList&lt;ApiOperation&gt; apiOperationSchemaMaps, bool includeProperties = True)

<hr /><div style='text-align: right'><i>Generated by MarkdownCodeDoc version 1.2</i></div>

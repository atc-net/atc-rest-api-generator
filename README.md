# ATC-NET REST API Generator

[![NuGet Version](https://img.shields.io/nuget/v/atc-rest-api-generator.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc-rest-api-generator)

## Table of content

- [ATC-NET REST API Generator](#atc-net-rest-api-generator)
  - [Table of content](#table-of-content)
  - [Projects in the repository](#projects-in-the-repository)
  - [Project dependency graph](#project-dependency-graph)
  - [CLI Tool](#cli-tool)
  - [Prerequisites](#prerequisites)
    - [Requirements](#requirements)
    - [Installation](#installation)
    - [Update](#update)
    - [Usage](#usage)
      - [Option **-h | --help**](#option--h----help)
      - [Option **generate server all -h**](#option-generate-server-all--h)
      - [Command **options-file**](#command-options-file)
      - [Default options-file - ApiGeneratorOptions.json](#default-options-file---apigeneratoroptionsjson)
      - [Custom options-file - ApiGeneratorOptions.json](#custom-options-file---apigeneratoroptionsjson)
      - [Options for locations explained](#options-for-locations-explained)
        - [Syntax](#syntax)
        - [Examples](#examples)
      - [Other options explained](#other-options-explained)
  - [PetStore Example](#petstore-example)
  - [Security - supporting role-based security and custom authentication-schemes](#security---supporting-role-based-security-and-custom-authentication-schemes)
    - [Roles and authentication-scheme validation](#roles-and-authentication-scheme-validation)
    - [Logic for determining `[Authorize]` or `[AllowAnonymous]` attributes](#logic-for-determining-authorize-or-allowanonymous-attributes)
    - [Example](#example)
  - [OpenApi Snippets](#openapi-snippets)
    - [List of items](#list-of-items)
    - [List of items with a custom PaginationResult](#list-of-items-with-a-custom-paginationresult)
    - [List of items with a built-in Pagination](#list-of-items-with-a-built-in-pagination)
    - [AsyncEnumerable of items](#asyncenumerable-of-items)
    - [AsyncEnumerable of items with a custom PaginationResult](#asyncenumerable-of-items-with-a-custom-paginationresult)
    - [AsyncEnumerable of items with a built-in Pagination](#asyncenumerable-of-items-with-a-built-in-pagination)
    - [Custom PaginationResult with query parameters](#custom-paginationresult-with-query-parameters)
  - [How to contribute](#how-to-contribute)

## Projects in the repository

|Project|Target Framework|Description|Nuget Download Link|
|---|---|---|---|
|[Atc.Rest.ApiGenerator](src/Atc.Rest.ApiGenerator) | net8.0 | Atc.Rest.ApiGenerator is a WebApi C# code generator using a OpenApi 3.0.x specification YAML file. | [![Nuget](https://img.shields.io/nuget/dt/Atc.Rest.ApiGenerator?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Atc.Rest.ApiGenerator) |
|[Atc.Rest.ApiGenerator.CLI](src/Atc.Rest.ApiGenerator.CLI) |net8.0 | A CLI tool that use Atc.Rest.ApiGenerator to create/update a project specified by a OpenApi 3.0.x specification YAML file. | [![Nuget](https://img.shields.io/nuget/dt/atc-rest-api-generator?logo=nuget&style=flat-square)](https://www.nuget.org/packages/atc-rest-api-generator) |
|[Atc.Rest.ApiGenerator.CodingRules](src/Atc.Rest.ApiGenerator.CodingRules) | net8.0| Create/update atc coding rules for the generated code | |
|[Atc.Rest.ApiGenerator.Contracts](src/Atc.Rest.ApiGenerator.Contracts) | net8.0| Shared contracts and interfaces for the generated code. | |
|[Atc.Rest.ApiGenerator.Framework.Mvc](src/Atc.Rest.ApiGenerator.Framework.Mvc) | net8.0| Provides support for generating ASP.NET MVC / Controller based REST API server implementations. | |
|[Atc.Rest.ApiGenerator.Framework.Minimal](src/Atc.Rest.ApiGenerator.Framework.Minimal) | net8.0| Provides support for generating MinimalAPI based REST server implementations. | |
|[Atc.Rest.ApiGenerator.Client.CSharp](src/Atc.Rest.ApiGenerator.Client.CSharp) | net8.0| Generates C# client code for interacting with the generated REST APIs. | |
|[Atc.Rest.ApiGenerator.Framework](src/Atc.Rest.ApiGenerator.Framework) | net8.0| Shared framework components and utilities for the API generator projects. | |
|[Atc.Rest.ApiGenerator.OpenApi](src/Atc.Rest.ApiGenerator.OpenApi) | net8.0| Handles OpenAPI specification parsing and manipulation for the API generator. | |
|[Atc.Rest.ApiGenerator.Nuget](src/Atc.Rest.ApiGenerator.Nuget) | net8.0| Manages NuGet packages required by the generated code and frameworks. | |
|[Atc.CodeGeneration.CSharp](src/Atc.CodeGeneration.CSharp) | net8.0| Provides utilities and functionalities for generating C# code. | |

## Project dependency graph

```mermaid
flowchart TB;
    CLI[Atc.Rest.ApiGenerator.CLI]
    Contracts[Atc.Rest.ApiGenerator.Contracts];
    ApiGenerator[Atc.Rest.ApiGenerator];
    CodingRules[Atc.Rest.ApiGenerator.CodingRules]
    ClientCSharp[Atc.Rest.ApiGenerator.Client.CSharp];
    ServerMvc[Atc.Rest.ApiGenerator.Framework.Mvc];
    ServerMinimal[Atc.Rest.ApiGenerator.Framework.Minimal];
    Framework[Atc.Rest.ApiGenerator.Framework];
    Nuget[Atc.Rest.ApiGenerator.Nuget];
    CSharpGenerator[Atc.CodeGeneration.CSharp];
    OpenApi[Atc.Rest.ApiGenerator.OpenApi];

    style CLI fill:#007FFF;
    style Contracts fill:#57A64A;
    style ApiGenerator fill:#;
    style CodingRules fill:#;
    style ClientCSharp fill:#B35A2D;
    style ServerMvc fill:#B35A2D;
    style ServerMinimal fill:#B35A2D;
    style Framework fill:#;
    style Nuget fill:#;
    style CSharpGenerator fill:#;
    style OpenApi fill:#;

    CLI --> ApiGenerator;
    CLI --> CodingRules;

    ApiGenerator --> ClientCSharp;
    ApiGenerator --> ServerMvc;
    ApiGenerator --> ServerMinimal;
    ApiGenerator .-> Contracts;

    ClientCSharp --> Framework;
    ClientCSharp .-> Contracts;
    ClientCSharp .-> CSharpGenerator;

    ServerMvc --> Framework;
    ServerMvc .-> Contracts;
    ServerMvc .-> CSharpGenerator;

    ServerMinimal --> Framework;
    ServerMinimal .-> Contracts;
    ServerMinimal .-> CSharpGenerator;

    Framework --> Nuget;
    Framework --> OpenApi;
    Framework .-> Contracts;
    Framework --> CSharpGenerator;

    OpenApi ..-> Contracts;
```

## CLI Tool

The `atc-rest-api-generator` is a cross platform command line application known as CLI tool.

The main purpose of this application is to create and maintain a REST-API based on an Open-API specification file using a `Design First` approach.

The `atc-rest-api-generator` can be categorized as a `Rapid Application Development Tool` for REST-API in .NET/C#.

## Prerequisites

To get the benefit out of this CLI tool, a `Design first` approach of the REST API in a `OpenAPI specification` should be in effect. The CLI tool needs a `OpenAPI specification` in `yaml` or `json` file format provided as the argument value for parameter `--specificationPath` or the short-hand-version `-s`.

Read more about REST API design in [ATC DevOps Playbook](https://atc-net.github.io/manuals/devops-playbook#rest-api-design).

Recommended tools for working with OpenAPI specifications:

- [Stoplight.io](https://stoplight.io/). Read more about it in the [Stoplight Documentation](https://meta.stoplight.io/) and get it from [Stoplight Download](https://stoplight.io/studio)
- [OpenAPI (Swagger) Editor](https://marketplace.visualstudio.com/items?itemName=42Crunch.vscode-openapi) extension for Visual Studio Code

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

### Installation

The tool can be installed as a .NET Core global tool by the following command

```powershell
dotnet tool install --global atc-rest-api-generator
```

or by following the instructions [here](https://www.nuget.org/packages/atc-rest-api-generator/) to install a specific version of the tool.

A successful installation will output something like

```powershell
The tool can be invoked by the following command: atc-rest-api-generator
Tool 'atc-rest-api-generator' (version '2.0.xxx') was successfully installed.`
```

### Update

The tool can be updated by following command

```powershell
dotnet tool update --global atc-rest-api-generator
```

### Usage

Since the tool is published as a .NET Tool, it can be launched from anywhere using any shell or command-line interface by calling **atc-rest-api-generator**. The help information is displayed when providing the `--help` argument to **atc-rest-api-generator**

#### Option **-h | --help**

```powershell
atc-rest-api-generator -h

USAGE:
    atc-rest-api-generator.exe [OPTIONS]

OPTIONS:
    -h, --help       Prints help information
        --verbose    Use verbose for more debug/trace information
        --version    Display version

COMMANDS:
    options-file    Commands for the options file 'ApiGeneratorOptions.json'
    generate        Operations related to generation of code
    validate        Operations related to validation of specifications
```

#### Option **generate server all -h**

```powershell
atc-rest-api-generator generate server all -h

DESCRIPTION:
Creates API, domain and host projects.

USAGE:
    atc-rest-api-generator.exe generate server all [OPTIONS]

EXAMPLES:
    atc-rest-api-generator.exe generate server -s c:\temp\MyProject\api.yml -p MyApi .
    atc-rest-api-generator.exe generate server all -s c:\temp\MyProject\api.yml -p MyApi .
    atc-rest-api-generator.exe generate server all -s c:\temp\MyProject\api.yml -p MyApi --outputSlnPath c:\temp\MyProject --outputSrcPath c:\temp\MyProject\src

OPTIONS:
    -h, --help                                                                    Prints help information
        --verbose                                                                 Use verbose for more debug/trace information
    -s, --specificationPath <SPECIFICATIONPATH>                                   Path to Open API specification (directory, file or url)
        --optionsPath [OPTIONSPATH]                                               Path to options json-file
        --validate-strictMode                                                     Use strictmode
        --validate-operationIdValidation                                          Use operationId validation
        --validate-operationIdCasingStyle [OPERATIONIDCASINGSTYLE]                Set casingStyle for operationId. Valid values are: CamelCase (default), KebabCase, PascalCase, SnakeCase
        --validate-modelNameCasingStyle [MODELNAMECASINGSTYLE]                    Set casingStyle for model name. Valid values are: CamelCase, KebabCase, PascalCase (default), SnakeCase
        --validate-modelPropertyNameCasingStyle [MODELPROPERTYNAMECASINGSTYLE]    Set casingStyle for model property name. Valid values are: CamelCase (default), KebabCase, PascalCase, SnakeCase
    -p, --projectPrefixName <PROJECTPREFIXNAME>                                   Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')
        --disableCodingRules                                                      Disable ATC-Coding-Rules
        --useProblemDetailsAsDefaultResponseBody                                  Use ProblemDetails as default responsen body
        --endpointsLocation [ENDPOINTSLOCATION]                                   If endpoints-localtion is provided, generated files will be placed here instead of the Endpoints folder
        --endpointsNamespace [ENDPOINTSNAMESPACE]                                 If endpoints-namespace is provided, generated files will be placed here instead of the Endpoints namespace
        --contractsLocation [CONTRACTSLOCATION]                                   If contracts-localtion is provided, generated files will be placed here instead of the Contracts folder
        --contractsNamespace [CONTRACTSNAMESPACE]                                 If contracts-namespace is provided, generated files will be placed here instead of the Contracts namespace
        --handlersLocation [HANDLERSLOCATION]                                     If handlers-localtion is provided, generated files will be placed here instead of the Handlers folder
        --handlersNamespace [HANDLERSNAMESPACE]                                   If handlers-namespace is provided, generated files will be placed here instead of the Handlers namespace
        --usePartialClassForContracts                                             Use Partial-Class for contracts
        --usePartialClassForEndpoints                                             Use Partial-Class for endpoints
        --removeNamespaceGroupSeparatorInGlobalUsings                             Remove space between namespace groups in GlobalUsing.cs
        --aspnet-output-type [ASPNETOUTPUTTYPE]                                   Set AspNet output type for the generated api. Valid values are: Mvc (default), MinimalApi
        --swagger-theme [SWAGGERTHEME]                                            Set Swagger theme for the hosting api. Valid values are: None, Default (default), Light, Dark
        --outputSlnPath <OUTPUTSLNPATH>                                           Path to solution file (directory or file)
        --outputSrcPath <OUTPUTSRCPATH>                                           Path to generated src projects (directory)
        --outputTestPath [OUTPUTTESTPATH]                                         Path to generated test projects (directory)
```

#### Command **options-file**

```powershell
USAGE:
    atc-rest-api-generator.exe options-file [OPTIONS] <COMMAND>

EXAMPLES:
    atc-rest-api-generator.exe options-file create
    atc-rest-api-generator.exe options-file validate

OPTIONS:
    -h, --help    Prints help information

COMMANDS:
    create      Create default options file 'ApiGeneratorOptions.json' if it doesn´t exist
    validate    Validate the options file 'ApiGeneratorOptions.json'
```

> **Note:** All values from the options-file will be overriden if pressent from the CLI options.
>
> **Example:** If the usePartialClassForContracts=false in the options-file and the CLI `--usePartialClassForContracts` options set, then the usePartialClassForContracts is true.

#### Default options-file - ApiGeneratorOptions.json

```json
{
  "generator": {
    "aspNetOutputType": "Mvc",
    "swaggerThemeMode": "None",
    "useRestExtended": true,
    "projectName": "",
    "projectSuffixName": "",
    "contractsLocation": "Contracts.[[apiGroupName]]",
    "contractsNamespace": "Contracts.[[apiGroupName]]",
    "endpointsLocation": "Endpoints.[[apiGroupName]]",
    "endpointsNamespace": "Endpoints.[[apiGroupName]]",
    "handlersLocation": "Handlers.[[apiGroupName]]",
    "handlersNamespace": "Handlers.[[apiGroupName]]",
    "usePartialClassForContracts": false,
    "usePartialClassForEndpoints": false,
    "removeNamespaceGroupSeparatorInGlobalUsings": false,
    "request": {},
    "response": {
      "useProblemDetailsAsDefaultBody": false
    }
  },
  "validation": {
    "strictMode": false,
    "operationIdValidation": false,
    "operationIdCasingStyle": "CamelCase",
    "modelNameCasingStyle": "PascalCase",
    "modelPropertyNameCasingStyle": "CamelCase"
  },
  "includeDeprecatedOperations": false
}
```

#### Custom options-file - ApiGeneratorOptions.json

```json
{
    "generator": {
      "aspNetOutputType": "MinimalApi",
      "swaggerThemeMode": "Dark",
      "useRestExtended": true,
      "projectName": "",
      "projectSuffixName": "",
      "contractsLocation": "Contracts.[[apiGroupName]]",
      "contractsNamespace": "Contracts.[[apiGroupName]]",
      "endpointsLocation": "Endpoints.[[apiGroupName]]",
      "endpointsNamespace": "Endpoints.[[apiGroupName]]",
      "handlersLocation": "Handlers.[[apiGroupName]]",
      "handlersNamespace": "Handlers.[[apiGroupName]]",
      "usePartialClassForContracts": false,
      "usePartialClassForEndpoints": false,
      "removeNamespaceGroupSeparatorInGlobalUsings": false,
      "request": {},
      "response": {
        "useProblemDetailsAsDefaultBody": false,
        "customErrorResponseModel": {
          "name": "ErrorResponse",
          "description": "Represents an error response.",
          "schema": {
            "status": {
              "dataType": "string?",
              "description": "Gets or sets the status of the error."
            },
            "message": {
              "dataType": "string?",
              "description": "Gets or sets the error message."
            },
            "readableMessage": {
              "dataType": "string?",
              "description": "Gets or sets the readable message."
            },
            "errorCode": {
              "dataType": "string?",
              "description": "Gets or sets the error code."
            },
            "context": {
              "dataType": "object?",
              "description": "Gets or sets the context information."
            }
          }
        }
      },
      "client": {
        "excludeEndpointGeneration": false,
        "httpClientName": "My-ApiClient"
      }
    },
    "validation": {
      "strictMode": false,
      "operationIdValidation": false,
      "operationIdCasingStyle": "CamelCase",
      "modelNameCasingStyle": "PascalCase",
      "modelPropertyNameCasingStyle": "CamelCase"
    },
    "includeDeprecatedOperations": false
}
```

#### Options for locations explained

The following options control the file locations for generated files such as contracts, endpoints, and handlers.
You can use specific syntax to define and customize the output file structure.

##### Syntax

For options like `contractsLocation`, `contractsNamespace`, `endpointsLocation`, `endpointsNamespace`, `handlersLocation`, `handlersNamespace`,
you can define paths using placeholders and custom directory names.

The syntax is flexible and allows you to organize files based on grouping or specific requirements.

##### Examples

| Option-Name | Option-Value | Example-file | Generated-output |
|-------------|--------------|--------------|------------------|
| contractsLocation  | Contracts                    | Account.cs | [Project-root]\Contracts\Accounts\Account.cs   |
| contractsLocation  | Contracts.[[apiGroupName]]   | Account.cs | [Project-root]\Contracts\Accounts\Account.cs   |
| contractsLocation  | Contracts-[[apiGroupName]]   | Account.cs | [Project-root]\Contracts\Accounts\Account.cs   |
| contractsLocation  | [[apiGroupName]].MyContracts | Account.cs | [Project-root]\Accounts\MyContracts\Account.cs |
| contractsLocation  | [[apiGroupName]]-MyContracts | Account.cs | [Project-root]\Accounts\MyContracts\Account.cs |
| contractsLocation  | [[apiGroupName]]             | Account.cs | [Project-root]\Accounts\Account.cs             |
| contractsLocation  | .                            | Account.cs | [Project-root]\Account.cs                      |
| contractsNamespace | Contracts                    | Account.cs | [Project-root].Contracts.Accounts.Account.cs   |
| contractsNamespace | Contracts.[[apiGroupName]]   | Account.cs | [Project-root].Contracts.Accounts.Account.cs   |
| contractsNamespace | Contracts-[[apiGroupName]]   | Account.cs | [Project-root].Contracts.Accounts.Account.cs   |
| contractsNamespace | [[apiGroupName]].MyContracts | Account.cs | [Project-root].Accounts.MyContracts.Account.cs |
| contractsNamespace | [[apiGroupName]]-MyContracts | Account.cs | [Project-root].Accounts.MyContracts.Account.cs |
| contractsNamespace | [[apiGroupName]]             | Account.cs | [Project-root].Accounts.Account.cs             |
| contractsNamespace | .                            | Account.cs | [Project-root].Account.cs                      |

> Placeholder Explanation:
>
> - [[apiGroupName]]: A placeholder replaced by the API group name during code generation. This allows grouping files dynamically based on your API structure.
> - [Project-root]: The root directory of your project where the generated files will be placed.

By using these options, you can effectively organize generated files into meaningful folder structures, ensuring clarity and scalability in your project layout.

#### Other options explained

The `projectSuffixName` extend `projectName` like the example:

| projectName | projectSuffixName | Generated project name | Reason                     |
|-------------|-------------------|------------------------|----------------------------|
| PetStore    |                   | PetStore.Api.Generated | default is `Api.Generated` |
| PetStore    | MyApi             | PetStore.MyApi         |                            |
| PetStore    | Foo.Api           | PetStore.Foo.Api       |                            |
| PetStore    | Bar-Api           | PetStore.Bar.Api       |                            |

## PetStore Example

The following command will generate an API that implements the offcial Pet Store example from Swagger.

```powershell
atc-rest-api-generator generate server all `
    --validate-strictMode false `
    -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml `
    -p PetStore `
    --outputSlnPath <MY_PROJECT_FOLDER> `
    --outputSrcPath <MY_PROJECT_FOLDER>\src `
    --outputTestPath <MMY_PROJECT_FOLDER>\test `
    --disableCodingRules `
    --verbose
```

Replace `<MY_PROJECT_FOLDER>` with an absolute path where the projects should be created. For example,
to put the generated solution in a folder called `C:\PetStore`, do the following:

```powershell
atc-rest-api-generator generate server all `
    --validate-strictMode false `
    -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml `
    -p PetStore `
    --outputSlnPath C:\PetStore `
    --outputSrcPath C:\PetStore\src `
    --outputTestPath C:\PetStore\test `
    --disableCodingRules `
    --verbose
```

The following is generated by running the above command:

```mermaid
flowchart TB;
    CMD[command: generate server all] --> HOST[Host project] & API[API project] & DOMAIN[Domain project];
    HOST-.-> API;
    API-.-> DOMAIN;
```

- The Host-project is the layer for the `.NET WebApi` application. Project suffix: `.Api`.
- The API-project is the layer with all the contracts, interfaces, result classes and endpoints. Project suffix: `.Api.Generated`.
- The Domain-project is the layer where handlers can be implemented with necessary business logic. Project suffix: `.Domain`.

Running the above command produces the following output:

```powershell
     _      ____    ___      ____                                        _
    / \    |  _ \  |_ _|    / ___|   ___   _ __     ___   _ __    __ _  | |_    ___    _ __
   / _ \   | |_) |  | |    | |  _   / _ \ | `_ \   / _ \ | `__|  / _` | | __|  / _ \  | `__|
  / ___ \  |  __/   | |    | |_| | |  __/ | | | | |  __/ | |    | (_| | | |_  | (_) | | |
 /_/   \_\ |_|     |___|    \____|  \___| |_| |_|  \___| |_|     \__,_|  \__|  \___/  |_|

🔽 Fetching api specification
     Download from: https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml
     Download time: 390.229 ms
🔍 Working on validation
     CR0103: Schema - Missing title on object type '#/components/schemas/Pet'.
     CR0101: Schema - Missing title on array type '#/components/schemas/Pets'.
     CR0103: Schema - Missing title on object type '#/components/schemas/Error'.
     CR0203: Operation - OperationId should start with the prefix 'Get' or 'List' for operation 'showPetById'.
     CR0214: Operation - Missing NotFound response type for operation 'ShowPetById', required by url parameter.
🔷 Working on server api generation (PetStore.Api.Generated)
     CR0801 - Old project does not exist
🟢   src:  PetStore.Api.Generated.csproj created
🟢   src:  ApiRegistration.cs created
🟢   src:  Contracts\Pets\Models\Error.cs created
🟢   src:  Contracts\Pets\Models\Pet.cs created
🟢   src:  Contracts\Pets\Models\Pets.cs created
🟢   src:  Contracts\Pets\Parameters\ListPetsParameters.cs created
🟢   src:  Contracts\Pets\Parameters\ShowPetByIdParameters.cs created
🟢   src:  Contracts\Pets\Results\ListPetsResult.cs created
🟢   src:  Contracts\Pets\Results\CreatePetsResult.cs created
🟢   src:  Contracts\Pets\Results\ShowPetByIdResult.cs created
🟢   src:  Contracts\Pets\Interfaces\IListPetsHandler.cs created
🟢   src:  Contracts\Pets\Interfaces\ICreatePetsHandler.cs created
🟢   src:  Contracts\Pets\Interfaces\IShowPetByIdHandler.cs created
🟢   src:  Endpoints\PetsController.cs created
🔷 Working on server domain generation (PetStore.Domain)
🟢   src:  PetStore.Domain.csproj created
🟢   src:  DomainRegistration.cs created
🟢   src:  Handlers\Pets\ListPetsHandler.cs created
🟢   src:  Handlers\Pets\CreatePetsHandler.cs created
🟢   src:  Handlers\Pets\ShowPetByIdHandler.cs created
🔶 Working on server domain unit-test generation (PetStore.Domain.Tests)
🟢   test: PetStore.Domain.Tests.csproj created
🟢   test: Handlers\Pets\Generated\ListPetsHandlerGeneratedTests.cs created
🟢   test: Handlers\Pets\ListPetsHandlerTests.cs created
🟢   test: Handlers\Pets\Generated\CreatePetsHandlerGeneratedTests.cs created
🟢   test: Handlers\Pets\CreatePetsHandlerTests.cs created
🟢   test: Handlers\Pets\Generated\ShowPetByIdHandlerGeneratedTests.cs created
🟢   test: Handlers\Pets\ShowPetByIdHandlerTests.cs created
🔷 Working on server host generation (PetStore.Api)
🟢   src:  PetStore.Api.csproj created
🟢   src:  Properties\launchSettings.json created
🟢   src:  Program.cs created
🟢   src:  Startup.cs created
🟢   src:  web.config created
🟢   src:  ConfigureSwaggerDocOptions.cs created
🔶 Working on server host unit-test generation (PetStore.Api.Tests)
🟢   test: PetStore.Api.Tests.csproj created
🟢   test: WebApiStartupFactory.cs created
🟢   test: WebApiControllerBaseTest.cs created
🟢   test: Endpoints\Pets\Generated\ListPetsHandlerStub.cs created
🟢   test: Endpoints\Pets\Generated\ListPetsTests.cs created
🟢   test: Endpoints\Pets\Generated\CreatePetsHandlerStub.cs created
🟢   test: Endpoints\Pets\Generated\CreatePetsTests.cs created
🟢   test: Endpoints\Pets\Generated\ShowPetByIdHandlerStub.cs created
🟢   test: Endpoints\Pets\Generated\ShowPetByIdTests.cs created
🟢   root: PetStore.sln created
🟢   root: PetStore.sln.DotSettings created
📐 Working on Coding Rules files
🟢   root: atc-coding-rules-updater.json created
🟢   root: atc-coding-rules-updater.ps1 created
🐭 Working on EditorConfig files
     Download from: [GitHub] /atc-net/atc-coding-rules/main/distribution/dotnet8/.editorconfig
     Download time: 27.947 ms
🟢   root: .editorconfig created
     Download from: [GitHub] /atc-net/atc-coding-rules/main/distribution/dotnet8/src/.editorconfig
     Download time: 22.987 ms
🟢   src: .editorconfig created
     Download from: [GitHub] /atc-net/atc-coding-rules/main/distribution/dotnet8/test/.editorconfig
     Download time: 24.465 ms
🟢   test: .editorconfig created
🔨 Working on Directory.Build.props files
     Download from: [GitHub] /atc-net/atc-coding-rules/main/distribution/dotnet8/Directory.Build.props
     Download time: 20.880 ms
🟢   root: Directory.Build.props created
     Download from: [GitHub] /atc-net/atc-coding-rules/main/distribution/dotnet8/src/Directory.Build.props
     Download time: 48.340 ms
🟢   src: Directory.Build.props created
     Download from: [GitHub] /atc-net/atc-coding-rules/main/distribution/dotnet8/test/Directory.Build.props
     Download time: 29.480 ms
🟢   test: Directory.Build.props created
✅ Done
```

After the generator finishes running, the API can be started by executing the following command:

```powershell
dotnet run --project C:\PetStore\src\PetStore.Api
```

And then open a browser and navigate to the url: [https://localhost:5001/swagger](https://localhost:5001/swagger)

## Security - supporting role-based security and custom authentication-schemes

To support role-based security and custom authentication-schemes, support is implemented for 3 custom extension tags in OpenApi specifications.

> x-authorize-roles (role array)
>
> x-authentication-schemes (auth-scheme array)
>
> x-authentication-required (true/false boolean)

At the root level of the specification the available roles and auth-schemes possible to use in paths (controllers) / path-items (actions/methods) should be specified. These are used to validate against defined roles other places in the specification.

### Roles and authentication-scheme validation

When reading the OpenApi specification, a lot of validations are being run against these 3 custom extension tags. E.g. validating that any path/path-item does not have roles and/or auth-schemes defined, which are not defined globally in the specification. Other validations are also in place to ensure that the combination of the 3 new extensions "tags" are set correctly.

### Logic for determining `[Authorize]` or `[AllowAnonymous]` attributes

The 3 extension "tags" can be specified at path/path-item levels.

If all path-items (operations) under a given path all have x-authentication-required set to true, then a [Authorize] header will still be added to the generated controller class. Otherwise [Authorize(Roles=x,y,z)] and [AllowAnonymous] will be applied the necessary places on the actions/methods in the controller.

Authentication-Schemes and Authorize-Roles defined at path/controller level is taken into consideration when generating [Authorize] attributes for path-item/action/method level.

If no path-items (operations) under a given path have the x-authentication-required extension set, then no attributes will be generated for that given path/controller. If you want to force e.g [Authorize] or [AllowAnonymous], set the x-authentication-required extension to `true` or `false` respectively.

### Example

> NOTE: Tags, parameters, responses, request-bodies, schemas etc. are removed for brevity, so the references in spec below are not valid - The specification is only illustrating the various places the 3 new extension tags can be applied.

```yaml
info:
  title: DEMO API
  description: DEMO API
  version: v1
  contact:
    name: ATC
  license:
    name: MIT
servers:
  - url: /api/v1
    description: Api version 1.0
x-authorize-roles:
  - api.execute.read
  - api.execute.write
  - admin
  - operator
x-authentication-schemes:
  - OpenIddict.Validation.AspNetCore
tags:
  - name: DEMO
    description: ''
paths:
  /data-templates:
    x-authentication-required: true
    x-authorize-roles: [operator]
    x-authentication-schemes: [OpenIddict.Validation.AspNetCore]
    get:
      summary: Returns a list of the groups data templates
      operationId: getDataTemplates
      x-authorize-roles: [admin,operator]
      x-authentication-schemes: [OpenIddict.Validation.AspNetCore]
    post:
      summary: Create a new data template
      operationId: createDataTemplate
      x-authentication-required: false
  '/data-templates/{dataTemplateId}':
    x-authentication-required: true
    x-authorize-roles: [operator]
    get:
      summary: Returns a specific data template
      operationId: getDataTemplateById
      x-authentication-required: true
      x-authorize-roles: [api.execute.read]
    delete:
      summary: Removes a specific data template
      operationId: deleteDataTemplateById
    put:
      summary: Updates a specific data template
      operationId: updateDataTemplateById
  '/data-templates/{dataTemplateId}/tags':
    post:
      summary: Creates a new data template tag
      operationId: createDataTemplateTag
      x-authorize-roles: [api.execute.read]
    delete:
      summary: Deletes a data template tag
      operationId: deleteDataTemplateTag
      x-authentication-schemes: [OpenIddict.Validation.AspNetCore]
  '/data-templates/{dataTemplateId}/tags/{dataTemplateTagId}':
    x-authentication-required: false
    put:
      summary: Updates a specific data template tag
      operationId: updateDataTemplateTagById
components:
  schemas: {}
  requestBodies: {}
  responses: {}
  securitySchemes: {}
```

## OpenApi Snippets

### List of items

```yaml
# Example with List of items
/users:
    get:
      operationId: getUsers
      responses:
        '200':
          description: OK
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/User'
```

```csharp
// Output for result type
IEnumerable<User>
```

### List of items with a custom PaginationResult

```yaml
 # Example with List of items with a custom PaginationResult
/users:
    get:
      operationId: getUsers
      responses:
        '200':
          description: success
          content:
            application/json:
              schema:
                allOf:
                  - $ref: '#/components/schemas/PaginatedResult'
                  - type: object
                    properties:
                      results:
                        type: array
                        items:
                            $ref: '#/components/schemas/User'
```

```csharp
// Output for result type
PaginationResult<User>
```

### List of items with a built-in Pagination

```yaml
# Example with List of items with a built-in Pagination
/users:
    get:
      operationId: getUsers
      responses:
        '200':
          description: success
          content:
            application/json:
              schema:
                allOf:
                  - $ref: '#/components/schemas/Pagination'
                  - $ref: '#/components/schemas/User'
```

```csharp
// Output for result type
Pagination<User>
```

### AsyncEnumerable of items

```yaml
# Example with AsyncEnumerable of items
/users:
    get:
      operationId: getUsers
      x-return-async-enumerable: true
      responses:
        '200':
          description: OK
          content:
            application/json:
              schema:
                type: array
                items:
                  $ref: '#/components/schemas/User'
```

```csharp
// Output for result type
IAsyncEnumerable<User>
```

### AsyncEnumerable of items with a custom PaginationResult

```yaml
# Example with AsyncEnumerable of items with a custom PaginationResult
/users:
    get:
      operationId: getUsers
      x-return-async-enumerable: true
      responses:
        '200':
          description: success
          content:
            application/json:
              schema:
                allOf:
                  - $ref: '#/components/schemas/PaginatedResult'
                  - type: object
                    properties:
                      results:
                        type: array
                        items:
                            $ref: '#/components/schemas/User'
```

```csharp
// Output for result type
IAsyncEnumerable<PaginationResult<User>>
```

### AsyncEnumerable of items with a built-in Pagination

```yaml
# Example with AsyncEnumerable of items with a built-in Pagination
# Note: The build-in Pagination can be found in namespace Atc.Rest.Results
/users:
    get:
      operationId: getUsers
      x-return-async-enumerable: true
      responses:
        '200':
          description: success
          content:
            application/json:
              schema:
                allOf:
                  - $ref: '#/components/schemas/Pagination'
                  - $ref: '#/components/schemas/User'
```

```csharp
// Output for result type
IAsyncEnumerable<Pagination<User>>
```

### Custom PaginationResult with query parameters

```yaml
 # Example with custom PaginationResult and use of query parameters
paths:
  /users:
    get:
      operationId: getUsers
      parameters:
        - $ref: '#/components/parameters/pageSize'
        - $ref: '#/components/parameters/continuationToken'
        - $ref: '#/components/parameters/queryString'
      responses:
        '200':
          description: success
          content:
            application/json:
              schema:
                allOf:
                  - $ref: '#/components/schemas/PaginationResult'
                  - type: object
                    properties:
                      results:
                        type: array
                        items:
                          $ref: '#/components/schemas/User'
components:
  schemas:
    PaginationResult:
      type: object
      title: PaginationResult
      description: An item result subset of a data query.
      properties:
        pageSize:
          type: integer
        continuationToken:
          type: string
          nullable: true
          description: Token to indicate next result set.
        items:
          type: array
          items: {}
  parameters:
    pageSize:
      name: pageSize
      in: query
      required: true
      schema:
        type: integer
        default: 10
    continuationToken:
      name: continuationToken
      in: query
      required: false
      schema:
        type: string
        nullable: true
    queryString:
      name: queryString
      in: query
      required: true
      schema:
        type: string
```

```csharp
// Output for generated parameter class in Minimal-Api
public record GetUsersParameters(
    [property: FromQuery] string? ContinuationToken,
    [property: FromQuery, Required] string QueryString,
    [property: FromQuery, Required] int PageSize = 10);


// Output for generated result class in Minimal-Api
public class GetUsersResult
{
    private GetUsersResult(IResult result)
    {
        Result = result;
    }

    public IResult Result { get; }

    /// <summary>
    /// 200 - Ok response.
    /// </summary>
    public static GetUsersResult Ok(PaginationResult<User> result)
        => new(TypedResults.Ok(result));

    /// <summary>
    /// Performs an implicit conversion from GetUsersResult to IResult.
    /// </summary>
    public static IResult ToIResult(GetUsersResult result)
        => result.Result;
}
```

## How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)
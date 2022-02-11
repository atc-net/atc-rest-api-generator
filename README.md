[![NuGet Version](https://img.shields.io/nuget/v/atc-rest-api-generator.svg?logo=nuget&style=for-the-badge)](https://www.nuget.org/packages/atc-rest-api-generator)

# ATC.Net Rest API Generator

## Breaking changes

![](https://img.shields.io/static/v1?color=ff9900&style=for-the-badge&label=&message=Breaking%20changes%20From%20Version%201.x%20to%202.x)

```powershell
 * CLI package renamed from 'atc-api-gen' to 'atc-rest-api-generator'
 * CLI tool renamed from 'atc-api' to 'atc-rest-api-generator'
 * atc-rest-api-generator validate schema command
    setting --strictMode renamed to --validate-strictMode
    setting --operationIdCasingStyle renamed to --validate-operationIdCasingStyle
    setting --modelNameCasingStyle renamed to --validate-modelNameCasingStyle
    setting --modelPropertyNameCasingStyle renamed to --validate-modelPropertyNameCasingStyle
 * Api-Options file
    Generator->UseNullableReferenceTypes has been removed (default in c# 10)
 ```

## Projects in the repository

|Project|Target Framework|Description|Docs|Nuget Download Link|
|---|---|---|---|---|
|[Atc.Rest.ApiGenerator](src/Atc.Rest.ApiGenerator)|net6.0|Atc.Rest.ApiGenerator is a WebApi C# code generator using a OpenApi 3.0.x specification YAML file.|[References](docs/CodeDoc/Atc.Rest.ApiGenerator/Index.md)<br/>[References extended](docs/CodeDoc/Atc.Rest.ApiGenerator/IndexExtended.md)|[![Nuget](https://img.shields.io/nuget/dt/Atc.Rest.ApiGenerator?logo=nuget&style=flat-square)](https://www.nuget.org/packages/Atc.Rest.ApiGenerator)|
|[Atc.Rest.ApiGenerator.CLI](src/Atc.Rest.ApiGenerator.CLI)|net6.0|A CLI tool that use Atc.Rest.ApiGenerator to create/update a project specified by a OpenApi 3.0.x specification YAML file.||[![Nuget](https://img.shields.io/nuget/dt/atc-rest-api-generator?logo=nuget&style=flat-square)](https://www.nuget.org/packages/atc-rest-api-generator)|

## CLI Tool

The Atc.Rest.ApiGenerator.CLI library is available through a cross platform command line application.

### Requirements

* [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)

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

#### Option <span style="color:yellow">-h | --help</span>
```powershell
atc-rest-api-generator -h

USAGE:
    atc-rest-api-generator.exe [OPTIONS]

OPTIONS:
    -h, --help       Prints help information
    -v, --verbose    Use verbose for more debug/trace information
        --version    Display version

COMMANDS:
    generate    Operations related to generation of code
    validate    Operations related to validation of specifications
```

#### Option <span style="color:yellow">generate server all -h</span>
```powershell
atc-rest-api-generator generate server all -h

USAGE:
    atc-rest-api-generator.exe generate server all [OPTIONS]

OPTIONS:
    -h, --help                                                                    Prints help information
    -v, --verbose                                                                 Use verbose for more debug/trace information
    -s, --specificationPath <SPECIFICATIONPATH>                                   Path to Open API specification (directory, file or url)
        --optionsPath [OPTIONSPATH]                                               Path to options json-file
        --validate-strictMode                                                     Use strictmode
        --validate-operationIdCasingStyle [OPERATIONIDCASINGSTYLE]                Set casingStyle for operationId. Valid values are: CamelCase (default), KebabCase, PascalCase, SnakeCase
        --validate-modelNameCasingStyle [MODELNAMECASINGSTYLE]                    Set casingStyle for model name. Valid values are: CamelCase, KebabCase, PascalCase (default), SnakeCase
        --validate-modelPropertyNameCasingStyle [MODELPROPERTYNAMECASINGSTYLE]    Set casingStyle for model property name. Valid values are: CamelCase (default), KebabCase, PascalCase, SnakeCase
        --useAuthorization                                                        Use authorization
    -p, --projectPrefixName <PROJECTPREFIXNAME>                                   Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated')
        --outputSlnPath <OUTPUTSLNPATH>                                           Path to solution file (directory or file)
        --outputSrcPath <OUTPUTSRCPATH>                                           Path to generated src projects (directory)
        --outputTestPath [OUTPUTTESTPATH]                                         Path to generated test projects (directory)
        --disableCodingRules                                                      Disable ATC-Coding-Rules
```

#### PetStore Example

The following command will generate an API that implements the offcial Pet Store example from Swagger.

```powershell
atc-rest-api-generator generate server all `
    --validate-strictMode `
    -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml `
    -p PetStore `
    --outputSlnPath <MY-PROJECT-FOLDER> `
    --outputSrcPath <MY-PROJECT-FOLDER>\src `
    --outputTestPath <MY-PROJECT-FOLDER>\test `
    -v
```

Replace `<MY-PROJECT-FOLDER>` with an absolute path where you want to projects created. For example,
to put the generated solution in a folder called `C:\PetStore`, do the following:

```powershell
atc-rest-api-generator generate server all `
    --validate-strictMode `
    -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml `
    -p PetStore `
    --outputSlnPath C:\PetStore `
    --outputSrcPath C:\PetStore\src `
    --outputTestPath C:\PetStore\test `
    -v
```

Running the above command produces the following output:

INSERTXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

After the generator is finished running, you can start the API by running the following command:

```
dotnet run --project C:\PetStore\src\PetStore.Api
```

And then open a browser with url: https://localhost:5001/swagger

## How to contribute

[Contribution Guidelines](https://atc-net.github.io/introduction/about-atc#how-to-contribute)

[Coding Guidelines](https://atc-net.github.io/introduction/about-atc#coding-guidelines)

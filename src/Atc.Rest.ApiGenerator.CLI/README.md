# CLI Tool Usage

The Atc.Rest.ApiGenerator.CLI library mentioned above is available through a cross platform command line application.

**Requirements**
- .NET Core 3.1 runtime

## Install:

The tool can be installed as a .NET Core global tool that you can call from the shell / command line
```
dotnet tool install --global atc-api-gen
```
or by following the instructions [here](https://www.nuget.org/packages/atc-api-gen/) to install a specific version of the tool.

A successful installation will output something like:
```
You can invoke the tool using the following command: atc-api
Tool 'atc-api-gen' (version '1.0.104') was successfully installed.`
```

## Update:

The tool can also be updated by following command.
```
dotnet tool update --global atc-api-gen
```

## Usage

Since the tool is published as a .NET Core Tool, it can be launched from anywhere using any command line interface by calling **atc-api**. The help information is displayed using the `--help` argument to **atc-api**

```
$ atc-api --help

Usage: atc-api [command] [options]

Commands
  generate
  validate
```

#### Example for --help on the commands 'generate server api'
```
$ atc-api generate server api --help

Create API project.

Usage: atc-api generate server api [options]

Options:
  --validate-strictMode                    Use strictMode
  --validate-operationIdCasingStyle        Set casingStyle for operationId
  --validate-modelNameCasingStyle          Set casingStyle for model name
  --validate-modelPropertyNameCasingStyle  Set casingStyle for model property name
  -p|--projectPrefixName                   Project prefix name (e.g. 'PetStore' becomes 'PetStore.Api.Generated').
  -o|--outputPath                          Path to generated project.
  --useNullableReferenceTypes              Use nullable reference types in .csproj
  --useAuthorization                       Use authorization
  -v|--verboseMode                         Use verboseMode for more debug/trace information
  --optionsPath                            Path to options json-file.
  -s|--specificationPath                   Path to Open API specification (directory, file or url)
  -?|-h|--help                             Show help information
```

#### PetStore example:

The following command will generate an API that implements the offcial Pet Store example from Swagger.

```
atc-api generate server all --validate-strictMode false -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml -p PetStore --outputSlnPath <MY-PROJECT-FOLDER> --outputSrcPath <MY-PROJECT-FOLDER>\src --outputTestPath <MY-PROJECT-FOLDER>\test -v true
```

Replace `<MY-PROJECT-FOLDER>` with an absolute path where you want to projects created. For example, 
to put the generated solution in a folder called `c:\PetStore`, do the following:

```
atc-api generate server all --validate-strictMode false -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml -p PetStore --outputSlnPath c:\PetStore --outputSrcPath c:\PetStore\src --outputTestPath c:\PetStore\test -v true
```

Running the command above produces the following output:

```
        ___  ______  _____        ___    ___    ____       _____                              __
       / _ |/_  __/ / ___/ ____  / _ |  / _ \  /  _/      / ___/ ___   ___  ___   ____ ___ _ / /_ ___   ____
      / __ | / /   / /__  /___/ / __ | / ___/ _/ /       / (_ / / -_) / _ \/ -_) / __// _ `// __// _ \ / __/
     /_/ |_|/_/    \___/       /_/ |_|/_/    /___/       \___/  \__/ /_//_/\__/ /_/   \_,_/ \__/ \___//_/


CR0103 # Warning: Schema - Missing title on object type '#/components/schemas/Pet'.
CR0101 # Warning: Schema - Missing title on array type '#/components/schemas/Pets'.
CR0103 # Warning: Schema - Missing title on object type '#/components/schemas/Error'.
CR0203 # Warning: Operation - OperationId should start with the prefix 'Get' or 'List' for operation 'ShowPetById'.
CR0214 # Warning: Operation - Missing NotFound response type for operation 'ShowPetById', required by url parameter.
CR0801 # Information: ProjectApiGenerated - Old project don't exist.
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\PetStore.Api.Generated.csproj
FileCreate # Debug: c:\PetStore\test\PetStore.Api.Generated.Tests\PetStore.Api.Generated.Tests.csproj
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Models\Error.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Models\Pet.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Models\Pets.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Parameters\ListPetsParameters.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Parameters\ShowPetByIdParameters.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Results\ListPetsResult.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Results\CreatePetsResult.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Results\ShowPetByIdResult.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Interfaces\IListPetsHandler.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Interfaces\ICreatePetsHandler.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Contracts\Pets\Interfaces\IShowPetByIdHandler.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api.Generated\Endpoints\PetsController.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Domain\PetStore.Domain.csproj
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\PetStore.Domain.Tests.csproj
FileCreate # Debug: c:\PetStore\src\PetStore.Domain\Handlers\Pets\ListPetsHandler.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Domain\Handlers\Pets\CreatePetsHandler.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Domain\Handlers\Pets\ShowPetByIdHandler.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\Handlers\Pets\Generated\ListPetsHandlerGeneratedTests.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\Handlers\Pets\ListPetsHandlerTests.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\Handlers\Pets\Generated\CreatePetsHandlerGeneratedTests.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\Handlers\Pets\CreatePetsHandlerTests.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\Handlers\Pets\Generated\ShowPetByIdHandlerGeneratedTests.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Domain.Tests\Handlers\Pets\ShowPetByIdHandlerTests.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api\PetStore.Api.csproj
FileCreate # Debug: c:\PetStore\src\PetStore.Api\Properties\launchSettings.json
FileCreate # Debug: c:\PetStore\src\PetStore.Api\Program.cs
FileCreate # Debug: c:\PetStore\src\PetStore.Api\Startup.cs
FileCreate # Debug: c:\PetStore\test\PetStore.Api.Tests\PetStore.Api.Tests.csproj
FileCreate # Debug: c:\PetStore\PetStore.sln

Server-API-Domain-Host is OK.
```

After the generator is finished running, you can start the API by running the following command:

```
dotnet run --project c:\PetStore\src\PetStore.Api
```

And then open a browser with url: https://localhost:5001/swagger

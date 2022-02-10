#### PetStore example:

The following command will generate an API that implements the offcial Pet Store example from Swagger.

```
atc-api generate server all --validate-strictMode -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml -p PetStore --outputSlnPath <MY-PROJECT-FOLDER> --outputSrcPath <MY-PROJECT-FOLDER>\src --outputTestPath <MY-PROJECT-FOLDER>\test -v
```

Replace `<MY-PROJECT-FOLDER>` with an absolute path where you want to projects created. For example,
to put the generated solution in a folder called `c:\PetStore`, do the following:

```
atc-api generate server all --validate-strictMode -s https://raw.githubusercontent.com/OAI/OpenAPI-Specification/master/examples/v3.0/petstore.yaml -p PetStore --outputSlnPath c:\PetStore --outputSrcPath c:\PetStore\src --outputTestPath c:\PetStore\test -v
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
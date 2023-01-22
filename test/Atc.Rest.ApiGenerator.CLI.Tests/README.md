# Atc.Rest.ApiGenerator.CLI.Tests

This xUnit test libary purpose is to execute the CLI tool as it was executed 
from the CLI with some different scenarios, defined by the `OpenApi yaml` file.

## ScenariosTests.cs

This is the class with the actual tests that points to the the folders with 
the `OpenApi yaml` file and `.verified.cs` files that is compared against the 
output from the CLI tool.

## Scenario - DemoSampleApi

This scenario contains a lot of different schemas and routes.
And is a copy of the sample project the can ce found [here](https://github.com/atc-net/atc-rest-api-generator/tree/main/sample).

## Scenario - DemoUsersApi

This scenario contains basic examples for `'CRUD' operations` for a `User` 
and `'R' operation` for the `Users`.

## Scenario - GenericPaginationApi

This scenario contains basic generic-pagination examples for `'R' operations` 
for `Cats` and `Dogs` with a `PaginatedResult`.

Defined schemaes:
- `Cat`
- `Dog`
- `PaginatedResult`

Defined endpoints:
- GET `/cats` => `PaginatedResult` + `Cat` with parameters that is defined as global-query-parameters.
- GET `/dogs` => `PaginatedResult` + `Dog` with parameters that is defined as global-query-parameters.

C# examples for EndpointResult's:

```csharp
PaginatedResult<Cat>

PaginatedResult<Dog>
```

Note for the request parameters, global-query-parameters is defined and ref from the request.
- `pageSize` as `int` and default set to `10`
- `pageIndex` as `int` and default set to `0`
- `queryString` as `string`

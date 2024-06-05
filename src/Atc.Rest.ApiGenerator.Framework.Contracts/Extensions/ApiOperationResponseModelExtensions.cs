namespace Atc.Rest.ApiGenerator.Framework.Contracts.Extensions;

public static class ApiOperationResponseModelExtensions
{
    public static IEnumerable<ApiOperationResponseModel> AppendUnauthorizedIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        ApiAuthorizeModel? authorization)
    {
        if (authorization is null)
        {
            return responseModels;
        }

        var models = responseModels.ToList();

        if (models.TrueForAll(x => x.StatusCode != HttpStatusCode.Unauthorized) &&
            !authorization.UseAllowAnonymous)
        {
            models.Add(
                new ApiOperationResponseModel(
                    HttpStatusCode.Unauthorized,
                    OperationName: "DummyOperation",
                    GroupName: null,
                    MediaType: null,
                    CollectionDataType: null,
                    DataType: null,
                    Description: null));
        }

        return models;
    }

    public static IEnumerable<ApiOperationResponseModel> AppendForbiddenIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        ApiAuthorizeModel? authorization)
    {
        if (authorization is null)
        {
            return responseModels;
        }

        var models = responseModels.ToList();

        if (models.TrueForAll(x => x.StatusCode != HttpStatusCode.Forbidden) &&
            authorization is
            {
                UseAllowAnonymous: false,
                Roles.Count: > 0
            })
        {
            models.Add(
                new ApiOperationResponseModel(
                    HttpStatusCode.Forbidden,
                    OperationName: "DummyOperation",
                    GroupName: null,
                    MediaType: null,
                    CollectionDataType: null,
                    DataType: null,
                    Description: null));
        }

        return models;
    }

    public static IEnumerable<ApiOperationResponseModel> AppendBadRequestIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        string? parameterTypeName)
    {
        var models = responseModels.ToList();

        if (models.TrueForAll(x => x.StatusCode != HttpStatusCode.BadRequest) &&
            parameterTypeName is not null)
        {
            models.Add(
                new ApiOperationResponseModel(
                    HttpStatusCode.BadRequest,
                    OperationName: "DummyOperation",
                    GroupName: null,
                    MediaType: null,
                    CollectionDataType: null,
                    DataType: null,
                    Description: null));
        }

        return models;
    }

    public static IEnumerable<ApiOperationResponseModel> AdjustNamespacesIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        IList<ApiOperation> operationSchemaMappings)
    {
        if (responseModels is null)
        {
            return Array.Empty<ApiOperationResponseModel>();
        }

        var models = new List<ApiOperationResponseModel>();
        foreach (var model in responseModels)
        {
            if (model.DataType is not null &&
                IsWellKnownSystemTypeName(model.DataType))
            {
                var operationSchemaMapping = operationSchemaMappings.First(x => x.Model.Name == model.DataType);
                if (operationSchemaMapping.Model.IsShared)
                {
                    models.Add(
                        model with
                        {
                            DataType = $"{ContentGeneratorConstants.Contracts}.{model.DataType}",
                        });
                }
                else
                {
                    models.Add(
                        model with
                        {
                            DataType = $"{ContentGeneratorConstants.Contracts}.{operationSchemaMapping.ApiGroupName}.{model.DataType}",
                        });
                }
            }
            else
            {
                models.Add(model);
            }
        }

        return models;
    }

    private static bool IsWellKnownSystemTypeName(
        string value)
        => value.EndsWith("Task", StringComparison.Ordinal) ||
           value.EndsWith("Tasks", StringComparison.Ordinal) ||
           value.EndsWith("Endpoint", StringComparison.Ordinal) ||
           value.EndsWith("EventArgs", StringComparison.Ordinal);
}
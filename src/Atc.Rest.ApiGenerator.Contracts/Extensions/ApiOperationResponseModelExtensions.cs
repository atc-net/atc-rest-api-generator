// ReSharper disable ConvertIfStatementToReturnStatement
namespace Atc.Rest.ApiGenerator.Contracts.Extensions;

public static class ApiOperationResponseModelExtensions
{
    public static string? GetQualifiedDataType(
        this ApiOperationResponseModel responseModel)
    {
        ArgumentNullException.ThrowIfNull(responseModel);

        if (string.IsNullOrEmpty(responseModel.DataType))
        {
            return null;
        }

        var dataType = responseModel.DataType;
        if (responseModel.Namespace is null)
        {
            return dataType;
        }

        if (responseModel.Namespace.EndsWith($".{responseModel.DataType}", StringComparison.Ordinal) ||
            responseModel.Namespace.Contains($"{responseModel.DataType}.", StringComparison.Ordinal))
        {
            return $"{responseModel.Namespace}.{dataType}";
        }

        // Special cases for .NET types
        if (dataType.Equals("Task", StringComparison.Ordinal))
        {
            return $"{responseModel.Namespace}.{dataType}";
        }

        return dataType;
    }

    public static IEnumerable<ApiOperationResponseModel> AppendUnauthorizedIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        ApiAuthorizeModel? authorization,
        bool isRequiredFromPath)
    {
        if (authorization is null &&
            !isRequiredFromPath)
        {
            return responseModels;
        }

        if (authorization is not null &&
            authorization.UseAllowAnonymous)
        {
            return responseModels;
        }

        var models = responseModels.ToList();

        var useAllowAnonymous = authorization?.UseAllowAnonymous ?? false;

        if (models.TrueForAll(x => x.StatusCode != HttpStatusCode.Unauthorized) &&
            (isRequiredFromPath || !useAllowAnonymous))
        {
            models.Add(
                new ApiOperationResponseModel(
                    HttpStatusCode.Unauthorized,
                    OperationName: "DummyOperation",
                    GroupName: null,
                    MediaType: null,
                    CollectionDataType: null,
                    DataType: null,
                    Description: null,
                    Namespace: null));
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
                    Description: null,
                    Namespace: null));
        }

        return models;
    }

    public static IEnumerable<ApiOperationResponseModel> AppendBadRequestIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        bool hasParameterType)
    {
        var models = responseModels.ToList();

        if (hasParameterType &&
            models.TrueForAll(x => x.StatusCode != HttpStatusCode.BadRequest))
        {
            models.Add(
                new ApiOperationResponseModel(
                    HttpStatusCode.BadRequest,
                    OperationName: "DummyOperation",
                    GroupName: null,
                    MediaType: null,
                    CollectionDataType: null,
                    DataType: null,
                    Description: null,
                    Namespace: null));
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
                    Description: null,
                    Namespace: null));
        }

        return models;
    }

    public static IEnumerable<ApiOperationResponseModel> AdjustNamespacesIfNeeded(
        this IEnumerable<ApiOperationResponseModel> responseModels,
        IList<ApiOperation> operationSchemaMappings,
        string contractNamespace)
    {
        if (responseModels is null ||
            contractNamespace is null)
        {
            return Array.Empty<ApiOperationResponseModel>();
        }

        var models = new List<ApiOperationResponseModel>();
        foreach (var model in responseModels)
        {
            if (model.DataType is not null &&
                model.DataType.IsWellKnownSystemTypeName())
            {
                var operationSchemaMapping = operationSchemaMappings.First(x => x.Model.Name == model.DataType);

                contractNamespace = operationSchemaMapping.Model.IsShared
                    ? contractNamespace.Replace("[[apiGroupName]]", string.Empty, StringComparison.Ordinal).Trim('.')
                    : contractNamespace.Replace("[[apiGroupName]]", operationSchemaMapping.ApiGroupName, StringComparison.Ordinal);

                models.Add(
                    model with
                    {
                        DataType = $"{contractNamespace}.{model.DataType}",
                    });
            }
            else
            {
                models.Add(model);
            }
        }

        return models;
    }
}
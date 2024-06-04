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
}
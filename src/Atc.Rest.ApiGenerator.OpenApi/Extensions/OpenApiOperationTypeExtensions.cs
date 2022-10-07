namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OpenApiOperationTypeExtensions
{
    public static HttpOperationType ToHttpOperationType(
        this OperationType operationType)
        => Enum<HttpOperationType>.Parse(operationType.ToString());
}
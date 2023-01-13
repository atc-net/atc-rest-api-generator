namespace Atc.Rest.ApiGenerator.OpenApi.Extensions;

public static class OperationTypeExtensions
{
    public static bool IsRequestBodySupported(
        this OperationType operationType)
        => operationType is OperationType.Patch or OperationType.Put or OperationType.Post;
}
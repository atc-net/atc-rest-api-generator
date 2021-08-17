// ReSharper disable CheckNamespace
namespace Microsoft.OpenApi.Models
{
    public static class OperationTypeExtensions
    {
        public static bool IsRequestBodySupported(this OperationType operationType)
        {
            return operationType is OperationType.Patch or OperationType.Put or OperationType.Post;
        }
    }
}
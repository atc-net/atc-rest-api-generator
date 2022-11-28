namespace Atc.Rest.ApiGenerator.Framework;

public interface IApiSpecificationContentReader
{
    OpenApiDocumentContainer CombineAndGetApiDocumentContainer(
        ILogger logger,
        string specificationPath);
}
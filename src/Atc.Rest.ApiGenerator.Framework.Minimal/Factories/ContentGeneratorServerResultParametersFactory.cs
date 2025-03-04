// ReSharper disable ConvertIfStatementToReturnStatement
// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Framework.Minimal.Factories;

public static class ContentGeneratorServerResultParametersFactory
{
    public static ContentGeneratorServerResultParameters Create(
        string @namespace,
        OpenApiOperation openApiOperation)
    {
        ArgumentNullException.ThrowIfNull(openApiOperation);

        var operationName = openApiOperation.GetOperationName();
        var httpStatusCodes = openApiOperation.Responses.GetHttpStatusCodes();

        // Methods
        var methodParameters = new List<ContentGeneratorServerResultMethodParameters>();
        var responseModels = openApiOperation.ExtractApiOperationResponseModels(@namespace).ToList();

        foreach (var responseModel in responseModels.OrderBy(x => x.StatusCode))
        {
            var httpStatusCode = responseModel.StatusCode;
            var documentationTags = new CodeDocumentationTags($"{(int)httpStatusCode} - {httpStatusCode.ToNormalizedString()} response.");

            methodParameters.Add(
                new ContentGeneratorServerResultMethodParameters(
                    documentationTags,
                    responseModel));
        }

        ContentGeneratorServerResultImplicitOperatorParameters? implicitOperatorParameters = null;

        var okResponseModel = responseModels.Find(x => x.StatusCode == HttpStatusCode.OK) ??
                              responseModels.Find(x => x.StatusCode == HttpStatusCode.Created);

        if (ShouldAppendImplicitOperatorContent(httpStatusCodes))
        {
            var collectionDataType = okResponseModel?.CollectionDataType;
            var dataType = okResponseModel?.DataType;

            // Implicit Operator
            implicitOperatorParameters = new ContentGeneratorServerResultImplicitOperatorParameters(
                CollectionDataType: collectionDataType,
                DataType: dataType);
        }

        return new ContentGeneratorServerResultParameters(
            @namespace,
            operationName,
            openApiOperation.ExtractDocumentationTagsForResult(),
            DeclarationModifiers.PublicClass,
            $"{operationName}{ContentGeneratorConstants.Result}",
            methodParameters,
            implicitOperatorParameters);
    }

    private static bool ShouldAppendImplicitOperatorContent(
        ICollection<HttpStatusCode> httpStatusCodes)
    {
        if (!httpStatusCodes.Contains(HttpStatusCode.OK) &&
            !httpStatusCodes.Contains(HttpStatusCode.Created))
        {
            return false;
        }

        return true;
    }
}
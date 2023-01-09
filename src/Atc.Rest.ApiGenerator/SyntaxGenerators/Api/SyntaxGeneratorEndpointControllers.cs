// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable UseDeconstruction
namespace Atc.Rest.ApiGenerator.SyntaxGenerators.Api;

public class SyntaxGeneratorEndpointControllers
{
    public SyntaxGeneratorEndpointControllers(
        ApiProjectOptions apiProjectOptions,
        IList<ApiOperation> operationSchemaMappings,
        string apiGroupName)
    {
        ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
        OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(operationSchemaMappings));
        ApiGroupName = apiGroupName ?? throw new ArgumentNullException(nameof(apiGroupName));
    }

    private ApiProjectOptions ApiProjectOptions { get; }

    private IList<ApiOperation> OperationSchemaMappings { get; }

    public string ApiGroupName { get; }

    public CompilationUnitSyntax? Code { get; private set; }

    public bool GenerateCode()
    {
        var compilationUnit = SyntaxFactory.CompilationUnit();
        Code = compilationUnit;
        return true;
    }

    public List<EndpointMethodMetadata> GetMetadataForMethods()
    {
        var list = new List<EndpointMethodMetadata>();
        foreach (var (key, value) in ApiProjectOptions.Document.GetPathsByBasePathSegmentName(ApiGroupName))
        {
            var generatorParameters = new SyntaxGeneratorContractParameters(ApiProjectOptions, ApiGroupName);
            var generatedParameters = generatorParameters.GenerateSyntaxTrees();
            var hasGlobalParameters = value.HasParameters();

            foreach (var apiOperation in value.Operations)
            {
                var httpAttributeRoutePart = GetHttpAttributeRoutePart(key);
                var routePart = string.IsNullOrEmpty(httpAttributeRoutePart)
                    ? $"/{GetRouteSegment()}"
                    : $"/{GetRouteSegment()}/{httpAttributeRoutePart}";
                var operationName = apiOperation.Value.GetOperationName();

                string? contractParameterTypeName = null;
                if (apiOperation.Value.HasParametersOrRequestBody() ||
                    value.HasParameters())
                {
                    contractParameterTypeName = operationName + NameConstants.ContractParameters;
                }

                var sgContractParameter = generatedParameters.FirstOrDefault(x => x.ApiOperation.GetOperationName() == operationName);

                var responseTypes = apiOperation.Value.Responses.GetResponseTypes(
                    OperationSchemaMappings,
                    ApiGroupName,
                    ApiProjectOptions.ProjectName,
                    useProblemDetailsAsDefaultResponseBody: false,
                    includeEmptyResponseTypes: false,
                    hasGlobalParameters || apiOperation.Value.HasParametersOrRequestBody(),
                    includeIfNotDefinedAuthorization: true,
                    includeIfNotDefinedInternalServerError: false);

                var responseTypeNamesAndItemSchema = GetResponseTypeNamesAndItemSchema(responseTypes);

                var endpointMethodMetadata = new EndpointMethodMetadata(
                    ApiProjectOptions.UseNullableReferenceTypes,
                    ApiProjectOptions.ProjectName,
                    ApiGroupName,
                    $"{ApiProjectOptions.RouteBase}{routePart}",
                    apiOperation.Key,
                    operationName,
                    "I" + operationName + NameConstants.ContractHandler,
                    contractParameterTypeName,
                    operationName + NameConstants.ContractResult,
                    responseTypeNamesAndItemSchema,
                    sgContractParameter?.ApiOperation,
                    sgContractParameter?.GlobalPathParameters,
                    ApiProjectOptions.Document.Components.Schemas,
                    OperationSchemaMappings);

                list.Add(endpointMethodMetadata);
            }
        }

        return list;
    }

    private string GetRouteSegment()
    {
        var (key, _) = ApiProjectOptions.Document.Paths
            .FirstOrDefault(x => x.IsPathStartingSegmentName(ApiGroupName));

        return key?
                   .Split('/', StringSplitOptions.RemoveEmptyEntries)?
                   .FirstOrDefault()
               ?? throw new NotSupportedException("SegmentName was not found in any route.");
    }

    private List<ResponseTypeNameAndItemSchema> GetResponseTypeNamesAndItemSchema(
        List<Tuple<HttpStatusCode, string>> responseTypeNames)
    {
        var list = new List<ResponseTypeNameAndItemSchema>();
        foreach (var responseTypeName in responseTypeNames)
        {
            if (string.IsNullOrEmpty(responseTypeName.Item2))
            {
                list.Add(new ResponseTypeNameAndItemSchema(responseTypeName.Item1, responseTypeName.Item2, null!));
            }
            else
            {
                var rawModelName = OpenApiDocumentSchemaModelNameResolver.GetRawModelName(responseTypeName.Item2);

                var isShared = OperationSchemaMappings.IsShared(rawModelName);
                var fullModelName = OpenApiDocumentSchemaModelNameResolver.EnsureModelNameWithNamespaceIfNeeded(
                    ApiProjectOptions.ProjectName,
                    ApiGroupName,
                    responseTypeName.Item2,
                    isShared);

                var schema = ApiProjectOptions.Document.Components.Schemas.FirstOrDefault(x => x.Key.Equals(rawModelName, StringComparison.OrdinalIgnoreCase));
                list.Add(new ResponseTypeNameAndItemSchema(responseTypeName.Item1, fullModelName, schema.Value));
            }
        }

        return list;
    }

    private static string GetHttpAttributeRoutePart(
        string urlPath)
    {
        var urlPathParts = urlPath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (urlPathParts.Length <= 1)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        for (var i = 1; i < urlPathParts.Length; i++)
        {
            if (i != 1)
            {
                sb.Append('/');
            }

            sb.Append(urlPathParts[i]);
        }

        return sb.ToString();
    }
}
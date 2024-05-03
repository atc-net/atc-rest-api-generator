// ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
namespace Atc.Rest.ApiGenerator.Client.CSharp.ContentGenerators;

public class ContentGeneratorClientEndpoint : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorClientEndpointParameters parameters;

    public ContentGeneratorClientEndpoint(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorClientEndpointParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.codeDocumentationTagsGenerator = codeDocumentationTagsGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTagsForClass))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTagsForClass));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.EndpointName} : {parameters.InterfaceName}");
        sb.AppendLine("{");
        sb.AppendLine(4, "private readonly IHttpClientFactory factory;");
        sb.AppendLine(4, "private readonly IHttpMessageFactory httpMessageFactory;");
        sb.AppendLine();
        sb.AppendLine(4, $"public {parameters.EndpointName}(");
        sb.AppendLine(8, "IHttpClientFactory factory,");
        sb.AppendLine(8, "IHttpMessageFactory httpMessageFactory)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "this.factory = factory;");
        sb.AppendLine(8, "this.httpMessageFactory = httpMessageFactory;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, $"public async Task<I{parameters.ResultName}> ExecuteAsync(");
        if (parameters.ParameterName is not null)
        {
            sb.AppendLine(8, $"{parameters.ParameterName} parameters,");
        }

        sb.AppendLine(8, $"string httpClientName = \"{parameters.HttpClientName}\",");
        sb.AppendLine(8, "CancellationToken cancellationToken = default)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "var client = factory.CreateClient(httpClientName);");
        sb.AppendLine();
        sb.AppendLine(8, $"var requestBuilder = httpMessageFactory.FromTemplate(\"{parameters.UrlPath}\");");

        if (parameters.Parameters is not null && parameters.Parameters.Any())
        {
            for (var index = 0; index < parameters.Parameters.Count; index++)
            {
                var item = parameters.Parameters[index];
                var isLastParameter = index == parameters.Parameters.Count - 1;

                switch (item.ParameterLocationType)
                {
                    case ParameterLocationType.Query:
                        AppendParameterForParameterLocation(sb, item, item.ParameterLocationType, isLastParameter);
                        break;
                    case ParameterLocationType.Header:
                        AppendParameterForParameterLocation(sb, item, item.ParameterLocationType, isLastParameter);
                        break;
                    case ParameterLocationType.Route:
                        sb.AppendLine(8, $"requestBuilder.WithPathParameter(\"{item.Name}\", parameters.{item.ParameterName});");
                        break;
                    case ParameterLocationType.Body:
                        sb.AppendLine(8, "requestBuilder.WithBody(parameters.Request);");
                        break;
                    case ParameterLocationType.Form:
                        sb.AppendLine(8, "// TODO: Imp. With-Form");
                        break;
                    default:
                        throw new SwitchCaseDefaultException(item.ParameterLocationType);
                }
            }
        }

        sb.AppendLine();
        sb.AppendLine(8, $"using var requestMessage = requestBuilder.Build(HttpMethod.{parameters.HttpMethod});");
        sb.AppendLine(8, "using var response = await client.SendAsync(requestMessage, cancellationToken);");
        sb.AppendLine();
        sb.AppendLine(8, "var responseBuilder = httpMessageFactory.FromResponse(response);");

        if (string.IsNullOrEmpty(parameters.SuccessResponseName))
        {
            sb.AppendLine(8, $"responseBuilder.AddSuccessResponse(HttpStatusCode.{parameters.SuccessResponseStatusCode});");
        }
        else
        {
            sb.AppendLine(
                8,
                parameters.UseListForModel
                    ? $"responseBuilder.AddSuccessResponse<List<{parameters.SuccessResponseName}>>(HttpStatusCode.OK);"
                    : $"responseBuilder.AddSuccessResponse<{parameters.SuccessResponseName}>(HttpStatusCode.{parameters.SuccessResponseStatusCode});");
        }

        foreach (var item in parameters.ErrorResponses)
        {
            sb.AppendLine(
                8,
                string.IsNullOrEmpty(item.ResponseType)
                    ? $"responseBuilder.AddErrorResponse(HttpStatusCode.{item.StatusCode});"
                    : $"responseBuilder.AddErrorResponse<{item.ResponseType}>(HttpStatusCode.{item.StatusCode});");
        }

        sb.AppendLine();
        sb.AppendLine(8, $"return await responseBuilder.BuildResponseAsync(x => new {parameters.ResultName}(x), cancellationToken);");
        sb.AppendLine(4, "}");
        sb.Append('}');

        return sb.ToString();
    }

    private static void AppendParameterForParameterLocation(
        StringBuilder sb,
        ContentGeneratorClientEndpointParametersParameters item,
        ParameterLocationType parameterLocationType,
        bool isLastParameter)
    {
        if (item.IsList)
        {
            sb.AppendLine(8, $"if (parameters.{item.ParameterName}.Any())");
            sb.AppendLine(8, "{");
            sb.AppendLine(12, $"requestBuilder.With{parameterLocationType}Parameter(\"{item.Name}\", parameters.{item.ParameterName});");
            sb.AppendLine(8, "}");

            if (!isLastParameter)
            {
                sb.AppendLine();
            }
        }
        else
        {
            sb.AppendLine(8, $"requestBuilder.With{parameterLocationType}Parameter(\"{item.Name}\", parameters.{item.ParameterName});");
        }
    }
}
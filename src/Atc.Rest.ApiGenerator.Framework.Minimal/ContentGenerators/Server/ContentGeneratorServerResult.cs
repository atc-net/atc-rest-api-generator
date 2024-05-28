namespace Atc.Rest.ApiGenerator.Framework.Minimal.ContentGenerators.Server;

public sealed class ContentGeneratorServerResult : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly CodeDocumentationTagsGenerator codeDocumentationTagsGenerator;
    private readonly ContentGeneratorServerResultParameters parameters;

    public ContentGeneratorServerResult(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        CodeDocumentationTagsGenerator codeDocumentationTagsGenerator,
        ContentGeneratorServerResultParameters parameters)
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
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(parameters.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(0, parameters.DocumentationTags));
        }

        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.ResultName}");
        sb.AppendLine("{");
        sb.AppendLine(4, $"private {parameters.ResultName}(IResult result)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "Result = result;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "public IResult Result { get; }");
        sb.AppendLine();

        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            AppendMethodContent(sb, item, parameters.ResultName);

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }

        if (parameters.ImplicitOperatorParameters is not null)
        {
            AppendImplicitOperatorContent(sb, parameters);
        }

        sb.Append('}');

        return sb.ToString();
    }

    private void AppendMethodContent(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        if (codeDocumentationTagsGenerator.ShouldGenerateTags(item.DocumentationTags))
        {
            sb.Append(codeDocumentationTagsGenerator.GenerateTags(4, item.DocumentationTags));
        }

        if (item.HttpStatusCode == HttpStatusCode.OK)
        {
            AppendMethodContentStatusCodeOk(sb, item, resultName);
        }
        else
        {
            AppendMethodContentForOtherStatusCodesThenOk(sb, item, resultName);
        }
    }

    private static void AppendImplicitOperatorContent(
        StringBuilder sb,
        ContentGeneratorServerResultParameters item)
    {
        if (item.ImplicitOperatorParameters is null)
        {
            return;
        }

        if (item.ImplicitOperatorParameters.SchemaType == SchemaType.SimpleType &&
            "object".Equals(item.ImplicitOperatorParameters.SimpleDataTypeName, StringComparison.Ordinal))
        {
            return;
        }

        sb.AppendLine();
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, $"/// Performs an implicit conversion from {item.ResultName} to IResult.");
        sb.AppendLine(4, "/// </summary>");
        sb.AppendLine(4, $"public static IResult ToIResult({item.ResultName} result)");
        sb.AppendLine(8, "=> result.Result;");
    }

    private static void AppendMethodContentStatusCodeOk(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        if (item.UsesBinaryResponse.HasValue &&
            item.UsesBinaryResponse.Value)
        {
            AppendMethodContentStatusCodeOkForBinary(sb, resultName);
        }
        else
        {
            if (string.IsNullOrEmpty(item.ModelName))
            {
                switch (item.SchemaType)
                {
                    case SchemaType.None:
                        sb.AppendLine(4, $"public static {resultName} Ok(string? message = null)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(message));");
                        break;
                    case SchemaType.SimpleType:
                        sb.AppendLine(4, $"public static {resultName} Ok({item.SimpleDataTypeName} response)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(response));");
                        break;
                    case SchemaType.SimpleTypeList:
                        sb.AppendLine(4, $"public static {resultName} Ok(IEnumerable<{item.SimpleDataTypeName}> result)");
                        sb.AppendLine(8, $"=> new(TypedResults.Ok(result ?? Enumerable.Empty<{item.SimpleDataTypeName}>()));");
                        break;
                    case SchemaType.SimpleTypePagedList:
                        sb.AppendLine(4, $"public static {resultName} Ok(Pagination<{item.SimpleDataTypeName}> result)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(result));");
                        break;
                    case SchemaType.SimpleTypeCustomPagedList:
                        sb.AppendLine(4, $"public static {resultName} Ok({item.GenericDataTypeName}<{item.SimpleDataTypeName}> result)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(result));");
                        break;
                    default:
                        sb.AppendLine("//// TODO: This is unexpected when we do not have a model-name!"); // TODO: Remove eventually
                        break;
                }
            }
            else
            {
                switch (item.SchemaType)
                {
                    case SchemaType.ComplexType:
                        sb.AppendLine(4, $"public static {resultName} Ok({item.ModelName} result)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(result));");
                        break;
                    case SchemaType.ComplexTypeList:
                        sb.AppendLine(4, $"public static {resultName} Ok(IEnumerable<{item.ModelName}> result)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(result));");
                        break;
                    case SchemaType.ComplexTypePagedList:
                        sb.AppendLine(4, $"public static {resultName} Ok(Pagination<{item.ModelName}> result)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(result));");
                        break;
                    case SchemaType.ComplexTypeCustomPagedList:
                        sb.AppendLine(4, $"public static {resultName} Ok({item.GenericDataTypeName}<{item.ModelName}> result)");
                        sb.AppendLine(8, "=> new(TypedResults.Ok(result));");
                        break;
                    default:
                        sb.AppendLine("//// TODO: This is unexpected when we have a model-name!"); // TODO: Remove eventually
                        break;
                }
            }
        }
    }

    private static void AppendMethodContentStatusCodeOkForBinary(
        StringBuilder sb,
        string resultName)
    {
        sb.AppendLine(4, $"public static {resultName} Ok(byte[] bytes, string fileName)");
        sb.AppendLine(8, $"=> new {resultName}(HEST.GRIS(bytes, fileName));");
    }

    private void AppendMethodContentForOtherStatusCodesThenOk(
        StringBuilder sb,
        ContentGeneratorServerResultMethodParameters item,
        string resultName)
    {
        switch (item.HttpStatusCode)
        {
            case HttpStatusCode.NoContent:
            case HttpStatusCode.Unauthorized:
                sb.AppendLine(4, $"public static {resultName} {item.HttpStatusCode}()");
                sb.AppendLine(8, $"=> new(TypedResults.{item.HttpStatusCode}());");
                break;
            case HttpStatusCode.Created:
            case HttpStatusCode.Accepted:
            case HttpStatusCode.BadRequest:
            case HttpStatusCode.NotFound:
            case HttpStatusCode.Conflict:
                sb.AppendLine(4, $"public static {resultName} {item.HttpStatusCode}(string? message = null)");
                sb.AppendLine(8, $"=> new(TypedResults.{item.HttpStatusCode}(message));");
                break;
            case HttpStatusCode.Forbidden:
                sb.AppendLine(4, $"public static {resultName} Forbid()");
                sb.AppendLine(8, "=> new(TypedResults.Forbid());");
                break;
            case HttpStatusCode.NotModified:
            case HttpStatusCode.MethodNotAllowed:
            case HttpStatusCode.InternalServerError:
            case HttpStatusCode.NotImplemented:
            case HttpStatusCode.BadGateway:
            case HttpStatusCode.ServiceUnavailable:
            case HttpStatusCode.GatewayTimeout:
                sb.AppendLine(4, $"public static {resultName} {item.HttpStatusCode}(string? message = null)");
                sb.AppendLine(8, $"=> new(TypedResults.Problem(new ProblemDetails {{ Detail = message, Status = (int)HttpStatusCode.{item.HttpStatusCode} }}));");
                break;
            default:
                sb.AppendLine($"// TODO: Not Implemented for {item.HttpStatusCode}.");
                break;
        }
    }
}
namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerController : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerControllerParameters parameters;

    public ContentGeneratorServerController(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerControllerParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();

        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Endpoint definitions.");
        sb.AppendLine($"/// Area: {parameters.Area}.");
        sb.AppendLine("/// </summary>");
        //// TODO: Authorize Attribute  [Authorize]
        sb.AppendLine("[ApiController]");
        sb.AppendLine($"[Route(\"{parameters.RouteBase}\")]");
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public class {parameters.Area}Controller : ControllerBase");
        sb.AppendLine("{");

        for (var i = 0; i < parameters.MethodParameters.Count; i++)
        {
            var item = parameters.MethodParameters[i];

            sb.AppendLine(4, "/// <summary>");
            sb.AppendLine(4, $"/// Description: {item.Description}");
            sb.AppendLine(4, $"/// Operation: {item.Name}.");
            sb.AppendLine(4, $"/// Area: {parameters.Area}.");
            sb.AppendLine(4, "/// </summary>");
            //// TODO: Authorize Attribute  [Authorize]
            sb.AppendLine(4, string.IsNullOrEmpty(item.RouteSuffix)
                ? $"[Http{item.OperationTypeRepresentation}]"
                : $"[Http{item.OperationTypeRepresentation}(\"{item.RouteSuffix}\")]");

            if (item.MultipartBodyLengthLimit.HasValue)
            {
                sb.AppendLine(4, item.MultipartBodyLengthLimit.Value == long.MaxValue
                    ? "[RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]"
                    : $"[RequestFormLimits(MultipartBodyLengthLimit = {item.MultipartBodyLengthLimit.Value})]");
            }

            foreach (var producesResponseTypeRepresentation in item.ProducesResponseTypeRepresentations)
            {
                sb.AppendLine(4, $"[{producesResponseTypeRepresentation}]");
            }

            sb.AppendLine(4, $"public async Task<ActionResult> {item.Name}(");

            if (!string.IsNullOrEmpty(item.ParameterTypeName))
            {
                sb.AppendLine(8, $"{item.ParameterTypeName} parameters,");
            }

            sb.AppendLine(8, $"[FromServices] {item.InterfaceName} handler,");
            sb.AppendLine(8, "CancellationToken cancellationToken)");

            sb.AppendLine(8, !string.IsNullOrEmpty(item.ParameterTypeName)
                    ? "=> await handler.ExecuteAsync(parameters, cancellationToken);"
                    : "=> await handler.ExecuteAsync(cancellationToken);");

            if (i < parameters.MethodParameters.Count - 1)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        return sb.ToString();
    }
}
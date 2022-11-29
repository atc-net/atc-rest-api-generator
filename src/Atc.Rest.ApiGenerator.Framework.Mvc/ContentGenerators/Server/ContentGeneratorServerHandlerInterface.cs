namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerHandlerInterface : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerHandlerInterfaceParameters parameters;

    public ContentGeneratorServerHandlerInterface(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerHandlerInterfaceParameters parameters)
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
        sb.AppendLine("/// Domain Interface for RequestHandler.");
        sb.AppendLine($"/// Description: {parameters.Description}");
        sb.AppendLine($"/// Operation: {parameters.OperationName}.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine($"public interface {parameters.InterfaceName}");
        sb.AppendLine("{");
        sb.AppendLine(4, "/// <summary>");
        sb.AppendLine(4, "/// Execute method.");
        sb.AppendLine(4, "/// </summary>");

        if (!string.IsNullOrEmpty(parameters.ParameterName))
        {
            sb.AppendLine(4, "/// <param name=\"parameters\">The parameters.</param>");
        }

        sb.AppendLine(4, "/// <param name=\"cancellationToken\">The cancellation token.</param>");
        sb.AppendLine(4, $"Task<{parameters.ResultName}> ExecuteAsync(");

        if (!string.IsNullOrEmpty(parameters.ParameterName))
        {
            sb.AppendLine(8, $"{parameters.ParameterName} parameters,");
        }

        sb.AppendLine(8, "CancellationToken cancellationToken = default);");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
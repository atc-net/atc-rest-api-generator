namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public class ContentGeneratorServerHandlerEnumeration : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerHandlerEnumerationParameters parameters;

    public ContentGeneratorServerHandlerEnumeration(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerHandlerEnumerationParameters parameters)
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
        sb.AppendLine(codeAttributeGenerator.Generate());
        if (parameters.UseFlagAttribute)
        {
            sb.AppendLine("[Flags]");
        }

        sb.AppendLine($"public enum {parameters.EnumerationName}");
        sb.AppendLine("{");

        foreach (var valueParameter in parameters.ValueParameters)
        {
            sb.AppendLine(
                4,
                valueParameter.Value is null
                    ? $"{valueParameter.Name},"
                    : $"{valueParameter.Name} = {valueParameter.Value},");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}
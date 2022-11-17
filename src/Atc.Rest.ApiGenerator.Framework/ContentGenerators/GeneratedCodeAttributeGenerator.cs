using Atc.Rest.ApiGenerator.Framework.Writers;

namespace Atc.Rest.ApiGenerator.Framework.ContentGenerators;

public class GeneratedCodeAttributeGenerator : IContentGenerator
{
    private readonly GeneratedCodeGeneratorParameters parameters;

    public GeneratedCodeAttributeGenerator(
        GeneratedCodeGeneratorParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
        => $"[GeneratedCode(\"{ContentWriterConstants.ApiGeneratorName}\", \"{parameters.ApiGeneratorVersion}\")]";
}
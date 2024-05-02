namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.ServerClient;

public static class GeneratedCodeHeaderGeneratorFactory
{
    public static GeneratedCodeHeaderGenerator Create(
        Version apiGeneratorVersion)
        => new(new GeneratedCodeGeneratorParameters(apiGeneratorVersion));
}
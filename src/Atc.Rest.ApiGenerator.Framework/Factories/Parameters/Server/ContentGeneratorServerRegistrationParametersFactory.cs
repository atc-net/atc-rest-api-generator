namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerRegistrationParametersFactory
{
    public static ContentGeneratorServerRegistrationParameters Create(
        string @namespace,
        string registrationPrefix)
        => new(
            @namespace,
            registrationPrefix + "Registration");
}
namespace Atc.Rest.ApiGenerator.Framework.Contracts.ContentGeneratorsParameters.Server;

public record ContentGeneratorServerRegistrationParameters(
    string Namespace,
    string RegistrationName);
namespace Atc.Rest.ApiGenerator.Framework.Factories.Parameters.Server;

public static class ContentGeneratorServerWebApiStartupFactoryParametersFactory
{
    public static ContentGeneratorServerWebApiStartupFactoryParameters Create(
        string @namespace)
    {
        var sbSummary = new StringBuilder();
        sbSummary.AppendLine("Factory for bootstrapping in memory tests.");
        sbSummary.AppendLine("Includes options to override configuration and service collection using a partial class.");
        var documentationTags = new CodeDocumentationTags(sbSummary.ToString());

        return new ContentGeneratorServerWebApiStartupFactoryParameters(
            @namespace,
            documentationTags);
    }
}
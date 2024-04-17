namespace Atc.Rest.ApiGenerator.Framework.Minimal.ProjectGenerator;

public class ServerDomainGenerator : IServerDomainGenerator
{
    public void GeneratedAssemblyMarker(
        ILogger logger,
        string projectName,
        Version projectVersion,
        DirectoryInfo path)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(projectName);
        ArgumentNullException.ThrowIfNull(projectVersion);
        ArgumentNullException.ThrowIfNull(path);

        var codeHeaderGenerator = new GeneratedCodeHeaderGenerator(
            new GeneratedCodeGeneratorParameters(
                projectVersion));
        var codeGeneratorContentHeader = codeHeaderGenerator.Generate();

        var codeGeneratorAttribute = new AttributeParameters(
            "GeneratedCode",
            $"\"{ContentWriterConstants.ApiGeneratorName}\", \"{projectVersion}\"");

        var suppressMessageAvoidEmptyInterfaceAttribute = new AttributeParameters(
            "SuppressMessage",
            "\"Design\", \"CA1040:Avoid empty interfaces\", Justification = \"OK.\"");

        var interfaceParameters = InterfaceParametersFactory.Create(
            codeGeneratorContentHeader,
            projectName,
            [suppressMessageAvoidEmptyInterfaceAttribute, codeGeneratorAttribute],
            "IDomainAssemblyMarker");

        var contentGeneratorInterface = new GenerateContentForInterface(
            new CodeDocumentationTagsGenerator(),
            interfaceParameters);

        var classContent = contentGeneratorInterface.Generate();

        var file = new FileInfo(
            Path.Combine(
                path.FullName,
                "IDomainAssemblyMarker.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            path,
            file,
            ContentWriterArea.Src,
            classContent);
    }
}
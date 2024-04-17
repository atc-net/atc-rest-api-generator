namespace Atc.Rest.ApiGenerator.Framework.Mvc.ProjectGenerator;

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

        var classParameters = ClassParametersFactory.Create(
            codeGeneratorContentHeader,
            projectName,
            codeGeneratorAttribute,
            "DomainRegistration");

        var contentGeneratorClass = new GenerateContentForClass(
            new CodeDocumentationTagsGenerator(),
            classParameters);

        var classContent = contentGeneratorClass.Generate();

        var file = new FileInfo(Path.Combine(
            path.FullName,
            "DomainRegistration.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            path,
            file,
            ContentWriterArea.Src,
            classContent);
    }
}
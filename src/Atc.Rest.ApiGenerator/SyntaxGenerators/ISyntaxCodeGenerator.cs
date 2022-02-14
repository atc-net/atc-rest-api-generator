namespace Atc.Rest.ApiGenerator.SyntaxGenerators;

public interface ISyntaxCodeGenerator
{
    public string FocusOnSegmentName { get; }

    CompilationUnitSyntax? Code { get; }

    bool GenerateCode();

    string ToCodeAsString();

    void ToFile();

    void ToFile(FileInfo file);
}
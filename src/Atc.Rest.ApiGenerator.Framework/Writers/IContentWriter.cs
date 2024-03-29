namespace Atc.Rest.ApiGenerator.Framework.Writers;

public interface IContentWriter
{
    void Write(
        DirectoryInfo workingFolder,
        FileInfo file,
        ContentWriterArea contentWriterArea,
        string content,
        bool overrideIfExist = true);
}
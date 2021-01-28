
using Xunit.Abstractions;

namespace Atc.Rest.ApiGenerator.Tests.Helpers.CodeGenerator
{
    public class YamlSpecFile: IXunitSerializable
    {
        public YamlSpecFile() { }

        public YamlSpecFile(string filePath, string fileName)
        {
            this.FilePath = filePath;
            this.FileName = fileName;
        }

        public string FilePath { get; private set; }

        public string FileName { get; private set; }

        public void Deserialize(IXunitSerializationInfo info)
        {
            FileName = info.GetValue<string>("fileName");
            FilePath = info.GetValue<string>("filePath");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("fileName", FileName);
            info.AddValue("filePath", FilePath);
        }

        public override string ToString()
        {
            return this.FileName;
        }
    }
}
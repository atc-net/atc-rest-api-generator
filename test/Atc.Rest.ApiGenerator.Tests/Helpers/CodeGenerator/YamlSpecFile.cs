using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Atc.Rest.ApiGenerator.Tests.Helpers.CodeGenerator
{
    public class YamlSpecFile : IXunitSerializable
    {
        public FileInfo FileInfo { get; private set; }

        public string FilePath => FileInfo.FullName;

        public string FileName => FileInfo.Name;

        public string DirectoryName => FileInfo.DirectoryName;

        public YamlSpecFile()
        {
        }

        public YamlSpecFile(FileInfo fileInfo) => this.FileInfo = fileInfo;

        public void Deserialize(IXunitSerializationInfo info) => FileInfo = new FileInfo(info.GetValue<string>("fileInfo"));

        public void Serialize(IXunitSerializationInfo info) => info.AddValue("fileInfo", FileInfo.FullName);

        public Task<string> LoadFileContentAsync() => FileInfo.OpenText().ReadToEndAsync();

        public override string ToString() => this.FileName;
    }
}
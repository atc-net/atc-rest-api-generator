using System;
using System.IO;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    public class ApiOptionsFile : IXunitSerializable
    {
        public FileInfo FileInfo { get; private set; }

        public string FilePath => FileInfo.FullName ?? throw new InvalidOperationException();

        public string FileName => FileInfo.Name ?? throw new InvalidOperationException();

        public string DirectoryName => FileInfo.DirectoryName ?? throw new InvalidOperationException();

        public ApiOptionsFile(FileInfo fileInfo) => this.FileInfo = fileInfo;

        public ApiOptionsFile()
        {
        }

        public void Deserialize(IXunitSerializationInfo info) => FileInfo = new FileInfo(info.GetValue<string>("fileInfo"));

        public void Serialize(IXunitSerializationInfo info) => info.AddValue("fileInfo", FileInfo.FullName);

        public Task<string> LoadFileContentAsync() => FileInfo.OpenText().ReadToEndAsync();

        public override string ToString() => this.FileName;
    }
}
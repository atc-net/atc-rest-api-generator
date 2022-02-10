namespace Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator
{
    public class GeneratorTestInput : IXunitSerializable
    {
        private FileInfo? yamlSpec;

        private FileInfo? generatorConfig;

        public Lazy<ApiOptions> GeneratorOptions { get; }

        public string TestName => yamlSpec is null ? string.Empty : Path.GetFileNameWithoutExtension(yamlSpec.Name);

        public string TestDirectory => yamlSpec?.DirectoryName ?? throw new InvalidOperationException();

        public GeneratorTestInput()
        {
            GeneratorOptions = new Lazy<ApiOptions>(() =>
            {
                var result = new ApiOptions();

                if (generatorConfig is not null)
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(generatorConfig.DirectoryName)
                        .AddJsonFile(generatorConfig.Name, optional: false, reloadOnChange: false)
                        .Build();

                    configuration.Bind(result);
                }

                return result;
            });
        }

        public GeneratorTestInput(
            FileInfo yamlSpec,
            FileInfo? generatorConfig = null)
            : this()
        {
            this.yamlSpec = yamlSpec;
            this.generatorConfig = generatorConfig;
        }

        public void Deserialize(
            IXunitSerializationInfo info)
        {
            yamlSpec = new FileInfo(info.GetValue<string>(nameof(yamlSpec)));

            if (info.GetValue<string>(nameof(generatorConfig)) is { } generatorConfigFileName)
            {
                generatorConfig = new FileInfo(generatorConfigFileName);
            }
        }

        public void Serialize(
            IXunitSerializationInfo info)
        {
            info.AddValue(nameof(yamlSpec), yamlSpec!.FullName);

            if (generatorConfig is not null)
            {
                info.AddValue(nameof(generatorConfig), generatorConfig.FullName);
            }
        }

        public Task<string> LoadYamlSpecContentAsync()
            => yamlSpec!.OpenText().ReadToEndAsync();

        public override string ToString()
            => this.TestName;
    }
}
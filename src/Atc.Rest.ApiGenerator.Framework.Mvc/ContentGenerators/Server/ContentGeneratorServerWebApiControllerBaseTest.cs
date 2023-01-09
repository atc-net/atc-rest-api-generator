namespace Atc.Rest.ApiGenerator.Framework.Mvc.ContentGenerators.Server;

public sealed class ContentGeneratorServerWebApiControllerBaseTest : IContentGenerator
{
    private readonly GeneratedCodeHeaderGenerator codeHeaderGenerator;
    private readonly GeneratedCodeAttributeGenerator codeAttributeGenerator;
    private readonly ContentGeneratorServerWebApiControllerBaseTestParameters parameters;

    public ContentGeneratorServerWebApiControllerBaseTest(
        GeneratedCodeHeaderGenerator codeHeaderGenerator,
        GeneratedCodeAttributeGenerator codeAttributeGenerator,
        ContentGeneratorServerWebApiControllerBaseTestParameters parameters)
    {
        this.codeHeaderGenerator = codeHeaderGenerator;
        this.codeAttributeGenerator = codeAttributeGenerator;
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.Append(codeHeaderGenerator.Generate());
        sb.AppendLine($"namespace {parameters.Namespace};");
        sb.AppendLine();
        sb.AppendLine(codeAttributeGenerator.Generate());
        sb.AppendLine("public abstract class WebApiControllerBaseTest : IClassFixture<WebApiStartupFactory>");
        sb.AppendLine("{");
        sb.AppendLine(4, "protected readonly WebApiStartupFactory Factory;");
        sb.AppendLine();
        sb.AppendLine(4, "protected readonly HttpClient HttpClient;");
        sb.AppendLine();
        sb.AppendLine(4, "protected readonly IConfiguration Configuration;");
        sb.AppendLine();
        sb.AppendLine(4, "protected static JsonSerializerOptions? JsonSerializerOptions;");
        sb.AppendLine();
        sb.AppendLine(4, "protected WebApiControllerBaseTest(WebApiStartupFactory fixture)");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "this.Factory = fixture;");
        sb.AppendLine(8, "this.HttpClient = this.Factory.CreateClient();");
        sb.AppendLine(8, "this.Configuration = new ConfigurationBuilder().Build();");
        sb.AppendLine(8, "JsonSerializerOptions = new JsonSerializerOptions");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "PropertyNameCaseInsensitive = true,");
        sb.AppendLine(12, "Converters =");
        sb.AppendLine(12, "{");
        sb.AppendLine(16, "new JsonStringEnumConverter()");
        sb.AppendLine(12, "},");
        sb.AppendLine(8, "};");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "protected static StringContent ToJson(object data)");
        sb.AppendLine(8, "=> new(JsonSerializer.Serialize(data, JsonSerializerOptions), Encoding.UTF8, \"application/json\");");
        sb.AppendLine();
        sb.AppendLine(4, "protected static StringContent Json(string data)");
        sb.AppendLine(8, "=> new(data, Encoding.UTF8, \"application/json\");");
        sb.AppendLine();
        sb.AppendLine(4, "protected static IFormFile GetTestFile()");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "var bytes = Encoding.UTF8.GetBytes(\"Hello World\");");
        sb.AppendLine(8, "var stream = new MemoryStream(bytes);");
        sb.AppendLine(8, "var formFile = new FormFile(stream, 0, stream.Length, \"dummy\", \"dummy.txt\")");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "Headers = new HeaderDictionary(),");
        sb.AppendLine(12, "ContentType = \"application/octet-stream\",");
        sb.AppendLine(8, "};");
        sb.AppendLine();
        sb.AppendLine(8, "return formFile;");
        sb.AppendLine(4, "}");
        sb.AppendLine();
        sb.AppendLine(4, "protected static List<IFormFile> GetTestFiles()");
        sb.AppendLine(8, "=> new() { GetTestFile(), GetTestFile() };");
        sb.AppendLine("}");

        return sb.ToString();
    }
}
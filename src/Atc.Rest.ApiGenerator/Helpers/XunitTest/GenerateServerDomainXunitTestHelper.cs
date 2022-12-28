namespace Atc.Rest.ApiGenerator.Helpers.XunitTest;

public static class GenerateServerDomainXunitTestHelper
{
    public static void GenerateGeneratedTests(
        ILogger logger,
        DomainProjectOptions domainProjectOptions,
        string apiGroupName,
        string handlerName,
        bool hasParametersOrRequestBody)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(domainProjectOptions);

        var nsTest = $"{domainProjectOptions.ProjectName}.Tests.{NameConstants.Handlers}.{apiGroupName}.Generated";

        var srcSyntaxNodeRoot = ReadCsFile(domainProjectOptions, apiGroupName, handlerName);
        var usedInterfacesInConstructor = GetUsedInterfacesInConstructor(srcSyntaxNodeRoot);

        var sb = new StringBuilder();

        GenerateCodeHelper.AppendGeneratedCodeWarningComment(sb, domainProjectOptions.ApiGeneratorNameAndVersion);
        sb.AppendLine($"namespace {nsTest}");
        sb.AppendLine("{");
        GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, domainProjectOptions.ApiGeneratorName, domainProjectOptions.ApiGeneratorVersion);
        sb.AppendLine(4, $"public class {handlerName}GeneratedTests");
        sb.AppendLine(4, "{");
        AppendInstantiateConstructor(sb, handlerName, usedInterfacesInConstructor);
        if (hasParametersOrRequestBody)
        {
            sb.AppendLine();
            AppendParameterArgumentNullCheck(sb, handlerName, usedInterfacesInConstructor);
        }

        sb.AppendLine(4, "}");
        sb.AppendLine("}");

        var pathGenerated = Path.Combine(Path.Combine(domainProjectOptions.PathForTestHandlers!.FullName, apiGroupName), "Generated");
        var fileGenerated = new FileInfo(Path.Combine(pathGenerated, $"{handlerName}GeneratedTests.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            domainProjectOptions.PathForTestGenerate!,
            fileGenerated,
            ContentWriterArea.Test,
            sb.ToString());
    }

    public static void GenerateCustomTests(
        ILogger logger,
        DomainProjectOptions domainProjectOptions,
        string apiGroupName,
        string handlerName)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(domainProjectOptions);

        var nsTest = $"{domainProjectOptions.ProjectName}.Tests.{NameConstants.Handlers}.{apiGroupName}";

        var sb = new StringBuilder();

        sb.AppendLine($"namespace {nsTest}");
        sb.AppendLine("{");
        sb.AppendLine(4, $"public class {handlerName}Tests");
        sb.AppendLine(4, "{");
        sb.AppendLine(8, "[Fact(Skip=\"Change this to a real test\")]");
        sb.AppendLine(8, "public void Sample()");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "// Arrange");
        sb.AppendLine();
        sb.AppendLine(12, "// Act");
        sb.AppendLine();
        sb.AppendLine(12, "// Assert");
        sb.AppendLine(8, "}");
        sb.AppendLine(4, "}");
        sb.AppendLine("}");

        var pathCustom = Path.Combine(domainProjectOptions.PathForTestHandlers!.FullName, apiGroupName);
        var fileCustom = new FileInfo(Path.Combine(pathCustom, $"{handlerName}Tests.cs"));

        var contentWriter = new ContentWriter(logger);
        contentWriter.Write(
            domainProjectOptions.PathForTestGenerate!,
            fileCustom,
            ContentWriterArea.Test,
            sb.ToString(),
            overrideIfExist: false);
    }

    private static void AppendInstantiateConstructor(
        StringBuilder sb,
        string handlerName,
        List<Tuple<string, string>> usedInterfacesInConstructor)
    {
        sb.AppendLine(8, "[Fact]");
        sb.AppendLine(8, "public void InstantiateConstructor()");
        sb.AppendLine(8, "{");
        if (usedInterfacesInConstructor.Count > 0)
        {
            sb.AppendLine(12, "// Arrange");
            foreach (var item in usedInterfacesInConstructor)
            {
                sb.AppendLine(12, $"var {item.Item2} = Substitute.For<{item.Item1}>();");
            }

            sb.AppendLine();
        }

        sb.AppendLine(12, "// Act");
        if (usedInterfacesInConstructor.Count > 0)
        {
            var parameters = string.Join(", ", usedInterfacesInConstructor.Select(x => x.Item2));
            sb.AppendLine(12, $"var actual = new {handlerName}({parameters});");
        }
        else
        {
            sb.AppendLine(12, $"var actual = new {handlerName}();");
        }

        sb.AppendLine();
        sb.AppendLine(12, "// Assert");
        sb.AppendLine(12, "Assert.NotNull(actual);");
        sb.AppendLine(8, "}");
    }

    private static void AppendParameterArgumentNullCheck(
        StringBuilder sb,
        string handlerName,
        List<Tuple<string, string>> usedInterfacesInConstructor)
    {
        sb.AppendLine(8, "[Fact]");
        sb.AppendLine(8, "public void ParameterArgumentNullCheck()");
        sb.AppendLine(8, "{");
        sb.AppendLine(12, "// Arrange");
        if (usedInterfacesInConstructor.Count > 0)
        {
            foreach (var item in usedInterfacesInConstructor)
            {
                sb.AppendLine(12, $"var {item.Item2} = Substitute.For<{item.Item1}>();");
            }

            sb.AppendLine();

            var parameters = string.Join(", ", usedInterfacesInConstructor.Select(x => x.Item2));
            sb.AppendLine(12, $"var sut = new {handlerName}({parameters});");
        }
        else
        {
            sb.AppendLine(12, $"var sut = new {handlerName}();");
        }

        sb.AppendLine();
        sb.AppendLine(12, "// Act & Assert");
        sb.AppendLine(12, "Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));");
        sb.AppendLine(8, "}");
    }

    private static SyntaxNode ReadCsFile(
        DomainProjectOptions domainProjectOptions,
        string area,
        string handlerName)
    {
        var csSrcFile = DirectoryInfoHelper.GetCsFileNameForHandler(domainProjectOptions.PathForSrcHandlers!, area, handlerName);
        var csSrcCode = File.ReadAllText(csSrcFile);
        var tree = CSharpSyntaxTree.ParseText(csSrcCode);
        return tree.GetRoot();
    }

    private static List<Tuple<string, string>> GetUsedInterfacesInConstructor(
        SyntaxNode root)
    {
        var constructorDeclarations = root.SelectToArray<ConstructorDeclarationSyntax>();
        if (constructorDeclarations.Length <= 0)
        {
            return new List<Tuple<string, string>>();
        }

        var parameterListSyntax = constructorDeclarations
            .First()
            .ParameterList;

        var parametersSyntax = parameterListSyntax
            .Select<ParameterSyntax>()
            .ToList();

        var list = new List<Tuple<string, string>>();
        foreach (var parameterSyntax in parametersSyntax)
        {
            var s = parameterSyntax.ToString();
            var lastSpaceIndex = s.LastIndexOf(' ');
            if (lastSpaceIndex == -1)
            {
                continue;
            }

            var s1 = s.Substring(0, lastSpaceIndex);
            var s2 = s.Substring(lastSpaceIndex + 1);
            list.Add(new Tuple<string, string>(s1, s2));
        }

        return list;
    }
}
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest;

public static class GenerateServerDomainXunitTestHelper
{
    public static void GenerateGeneratedTests(
        ILogger logger,
        DomainProjectOptions domainProjectOptions,
        SyntaxGeneratorHandler sgHandler)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(domainProjectOptions);
        ArgumentNullException.ThrowIfNull(sgHandler);

        var area = sgHandler.FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var nsSrc = $"{domainProjectOptions.ProjectName}.{NameConstants.Handlers}.{area}";
        var nsTest = $"{domainProjectOptions.ProjectName}.Tests.{NameConstants.Handlers}.{area}.Generated";

        var srcSyntaxNodeRoot = ReadCsFile(domainProjectOptions, sgHandler.FocusOnSegmentName, sgHandler);
        var usedInterfacesInConstructor = GetUsedInterfacesInConstructor(srcSyntaxNodeRoot);

        var usingStatements = GetUsedUsingStatements(
            srcSyntaxNodeRoot,
            nsSrc,
            usedInterfacesInConstructor.Count > 0);

        var sb = new StringBuilder();
        foreach (var item in usingStatements)
        {
            sb.AppendLine($"using {item};");
        }

        sb.AppendLine();
        GenerateCodeHelper.AppendGeneratedCodeWarningComment(sb, domainProjectOptions.ToolNameAndVersion);
        sb.AppendLine($"namespace {nsTest}");
        sb.AppendLine("{");
        GenerateCodeHelper.AppendGeneratedCodeAttribute(sb, domainProjectOptions.ToolName, domainProjectOptions.ToolVersion);
        sb.AppendLine(4, $"public class {sgHandler.HandlerTypeName}GeneratedTests");
        sb.AppendLine(4, "{");
        AppendInstantiateConstructor(sb, sgHandler, usedInterfacesInConstructor);
        if (sgHandler.HasParametersOrRequestBody)
        {
            sb.AppendLine();
            AppendParameterArgumentNullCheck(sb, sgHandler, usedInterfacesInConstructor);
        }

        sb.AppendLine(4, "}");
        sb.AppendLine("}");

        var pathGenerated = Path.Combine(Path.Combine(domainProjectOptions.PathForTestHandlers!.FullName, area), "Generated");
        var fileGenerated = new FileInfo(Path.Combine(pathGenerated, $"{sgHandler.HandlerTypeName}GeneratedTests.cs"));

        var fileDisplayLocation = fileGenerated.FullName.Replace(domainProjectOptions.PathForTestGenerate!.FullName, "test: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, fileGenerated, fileDisplayLocation, sb.ToString());
    }

    public static void GenerateCustomTests(
        ILogger logger,
        DomainProjectOptions domainProjectOptions,
        SyntaxGeneratorHandler sgHandler)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(domainProjectOptions);
        ArgumentNullException.ThrowIfNull(sgHandler);

        var area = sgHandler.FocusOnSegmentName.EnsureFirstCharacterToUpper();
        var nsTest = $"{domainProjectOptions.ProjectName}.Tests.{NameConstants.Handlers}.{area}";

        var sb = new StringBuilder();
        sb.AppendLine("using Xunit;");
        sb.AppendLine();
        sb.AppendLine($"namespace {nsTest}");
        sb.AppendLine("{");
        sb.AppendLine(4, $"public class {sgHandler.HandlerTypeName}Tests");
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

        var pathCustom = Path.Combine(domainProjectOptions.PathForTestHandlers!.FullName, area);
        var fileCustom = new FileInfo(Path.Combine(pathCustom, $"{sgHandler.HandlerTypeName}Tests.cs"));

        var fileDisplayLocation = fileCustom.FullName.Replace(domainProjectOptions.PathForTestGenerate!.FullName, "test: ", StringComparison.Ordinal);
        TextFileHelper.Save(logger, fileCustom, fileDisplayLocation, sb.ToString(), overrideIfExist: false);
    }

    private static void AppendInstantiateConstructor(
        StringBuilder sb,
        SyntaxGeneratorHandler sgHandler,
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
            sb.AppendLine(12, $"var actual = new {sgHandler.HandlerTypeName}({parameters});");
        }
        else
        {
            sb.AppendLine(12, $"var actual = new {sgHandler.HandlerTypeName}();");
        }

        sb.AppendLine();
        sb.AppendLine(12, "// Assert");
        sb.AppendLine(12, "Assert.NotNull(actual);");
        sb.AppendLine(8, "}");
    }

    private static void AppendParameterArgumentNullCheck(
        StringBuilder sb,
        SyntaxGeneratorHandler sgHandler,
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
            sb.AppendLine(12, $"var sut = new {sgHandler.HandlerTypeName}({parameters});");
        }
        else
        {
            sb.AppendLine(12, $"var sut = new {sgHandler.HandlerTypeName}();");
        }

        sb.AppendLine();
        sb.AppendLine(12, "// Act & Assert");
        sb.AppendLine(12, "Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));");
        sb.AppendLine(8, "}");
    }

    private static SyntaxNode ReadCsFile(
        DomainProjectOptions domainProjectOptions,
        string area,
        SyntaxGeneratorHandler sgHandler)
    {
        var csSrcFile = Util.GetCsFileNameForHandler(domainProjectOptions.PathForSrcHandlers!, area, sgHandler.HandlerTypeName);
        var csSrcCode = File.ReadAllText(csSrcFile);
        var tree = CSharpSyntaxTree.ParseText(csSrcCode);
        return tree.GetRoot();
    }

    private static List<string> GetUsedUsingStatements(
        SyntaxNode root,
        string nsSrc,
        bool useExtra)
    {
        var list = new List<string>
        {
            nsSrc,
            "System",
            "System.CodeDom.Compiler",
            "Xunit",
        };

        if (useExtra)
        {
            list.Add("NSubstitute");

            var usingDirective = root.GetUsedUsingStatementsWithoutAlias();
            foreach (var item in usingDirective)
            {
                if (item.StartsWith("System", StringComparison.Ordinal))
                {
                    continue;
                }

                list.Add(item);
            }
        }

        var usings = new List<string>();
        usings.AddRange(
            list
                .Where(x => x.StartsWith("System", StringComparison.Ordinal))
                .OrderBy(x => x));
        usings.AddRange(
            list
                .Where(x => !x.StartsWith("System", StringComparison.Ordinal))
                .OrderBy(x => x));
        return usings;
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
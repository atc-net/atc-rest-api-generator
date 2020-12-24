using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Domain;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class GenerateServerDomainXunitTestHelper
    {
        public static LogKeyValueItem GenerateGeneratedTests(
            DomainProjectOptions domainProjectOptions,
            SyntaxGeneratorHandler sgHandler)
        {
            if (domainProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(domainProjectOptions));
            }

            if (sgHandler == null)
            {
                throw new ArgumentNullException(nameof(sgHandler));
            }

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
            GenerateCodeHelper.AppendNamespaceComment(sb, domainProjectOptions.ToolNameAndVersion);
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
            return TextFileHelper.Save(fileGenerated, sb.ToString());
        }

        public static LogKeyValueItem GenerateCustomTests(
            DomainProjectOptions domainProjectOptions,
            SyntaxGeneratorHandler sgHandler)
        {
            if (domainProjectOptions == null)
            {
                throw new ArgumentNullException(nameof(domainProjectOptions));
            }

            if (sgHandler == null)
            {
                throw new ArgumentNullException(nameof(sgHandler));
            }

            var area = sgHandler.FocusOnSegmentName.EnsureFirstCharacterToUpper();
            var nsSrc = $"{domainProjectOptions.ProjectName}.{NameConstants.Handlers}.{area}";
            var nsTest = $"{domainProjectOptions.ProjectName}.Tests.{NameConstants.Handlers}.{area}";

            var sb = new StringBuilder();
            sb.AppendLine($"using {nsSrc};");
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
            return TextFileHelper.Save(fileCustom, sb.ToString(), false);
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

        private static SyntaxNode ReadCsFile(DomainProjectOptions domainProjectOptions, string area, SyntaxGeneratorHandler sgHandler)
        {
            var csSrcFile = Util.GetCsFileNameForHandler(domainProjectOptions.PathForSrcHandlers!, area, sgHandler.HandlerTypeName);
            var csSrcCode = File.ReadAllText(csSrcFile);
            var tree = CSharpSyntaxTree.ParseText(csSrcCode);
            return tree.GetRoot();
        }

        private static List<string> GetUsedUsingStatements(SyntaxNode root, string nsSrc, bool useExtra)
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

            return list
                .OrderBy(x => x)
                .ToList();
        }

        private static List<Tuple<string, string>> GetUsedInterfacesInConstructor(SyntaxNode root)
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

            var interfaceNames = new List<string>();
            if (parametersSyntax.Count > 0)
            {
                interfaceNames.AddRange(parametersSyntax
                    .Select(p => p.Select<IdentifierNameSyntax>()
                        .Select(x => x.Identifier.Text)
                        .ToArray())
                    .Select(sa => sa.Length == 1
                        ? sa[0]
                        : string.Join('.', sa)));
            }

            var interfaceGenericNames = parameterListSyntax
                .Select<GenericNameSyntax>()
                .Select(x => x.GetText().ToString().Trim())
                .ToArray();

            var names = parameterListSyntax
                .Select<ParameterSyntax>()
                .Select(x => x.Identifier.Text)
                .ToArray();

            if (interfaceNames.Count <= 0 || interfaceNames.Count != names.Length)
            {
                return new List<Tuple<string, string>>();
            }

            if (interfaceGenericNames.Length == 0)
            {
                return interfaceNames
                    .Select((t, i) => new Tuple<string, string>(t, names[i]))
                    .ToList();
            }

            var list = new List<Tuple<string, string>>();
            for (int i = 0; i < interfaceNames.Count; i++)
            {
                var interfaceGenericName = interfaceGenericNames.FirstOrDefault(x => x.Contains($"<{interfaceNames[i]}>", StringComparison.Ordinal));
                list.Add(string.IsNullOrEmpty(interfaceGenericName)
                    ? new Tuple<string, string>(interfaceNames[i], names[i])
                    : new Tuple<string, string>(interfaceGenericName, names[i]));
            }

            return list;
        }
    }
}
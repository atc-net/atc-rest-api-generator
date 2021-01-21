using System.Linq;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.Helpers;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api.ContractModel
{
    public class ContractModelEnumTest
    {
        private static readonly string ProjectPrefix = "TestProject";

        private static readonly string ProjectSuffix = "AtcTest";

        private static readonly string FocusOnSecment = "Test";

        private readonly string spec = @"
openapi: 3.0.0
components:
  schemas:
    EnumComponent:
      title: EnumComponent
      description: 'A enum component'
      type: object
      properties:
        enumProperty:
          type: string
          description: 'enum of with two items'
          enum:
            - Item1
            - Item2
".Trim();

        private readonly string expectedCode = @$"
{GeneratorOutputUsings.UsingCodeAnalysis}

{GeneratorOutput.GeneratorAtcComment}
namespace {ProjectPrefix}.{ProjectSuffix}.Contracts
{{
    {GeneratorOutputSuppressMessages.SuppressTrailingCommaMessage}
    public enum EnumComponent
    {{
        Item1,
        Item2
    }}
}}
".Trim();

        [Fact]
        public void TestSpecModelGenerator()
        {
            // Setup
            var apiProj = GeneratorTestSetup.CreateApiProject(spec, ProjectPrefix, ProjectSuffix);
            var sut = new SyntaxGeneratorContractModel(apiProj, string.Empty, apiProj.Document.Components.Schemas.First().Value, FocusOnSecment);

            // Act
            var specOutput = sut.ToCodeAsString();

            // Assert
            Assert.Equal(expectedCode, specOutput);
        }
    }
}
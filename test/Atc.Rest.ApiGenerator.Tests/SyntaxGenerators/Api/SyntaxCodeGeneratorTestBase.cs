using System.Threading.Tasks;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators;
using Atc.Rest.ApiGenerator.Tests.XUnitTestTypes.CodeGenerator;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api
{
    public abstract class SyntaxCodeGeneratorTestBase : GeneratorTestBase
    {
        protected abstract ISyntaxCodeGenerator CreateGenerator(ApiProjectOptions apiProject);

        protected async Task VerifyGeneratedOutput(GeneratorTestInput input)
        {
            // Arrange
            var apiProject = await CreateApiProjectAsync(input);
            var verifySettings = CreateVerifySettings(input, apiProject);

            var sut = CreateGenerator(apiProject);

            // Act
            var generatedCode = sut.ToCodeAsString();

            // Assert
            await VerifyGeneratedCode(generatedCode, verifySettings);
        }
    }
}
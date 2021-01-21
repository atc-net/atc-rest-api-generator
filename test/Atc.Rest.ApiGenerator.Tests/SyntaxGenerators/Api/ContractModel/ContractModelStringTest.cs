using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Atc.Rest.ApiGenerator.Helpers;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Atc.Rest.ApiGenerator.Tests.Helpers;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators.Api.ContractModel
{
    public class ContractModelStringTest
    {
        private static readonly string ProjectPrefix = "TestProject";

        private static readonly string ProjectSuffix = "AtcTest";

        private static readonly string FocusOnSecment = "Test";


        private readonly string spec = @"
openapi: 3.0.0
components:
  schemas:
    stringComponent:
      title: StringComponent
      description: 'A string component'
      type: object
      properties:
        stringProperty:
          type: string
          description: 'normal string'
        uriProperty:
          type: string
          format: uri
          description: 'uri string'
        uuidProperty:
          type: string
          format: uuid
          description: 'uuid string'
      additionalProperties: false
".Trim();

        private readonly string expectedCode = @$"
{GeneratorOutputUsings.UsingSystem}
{GeneratorOutputUsings.UsingCodeDomCompiler}
{GeneratorOutputUsings.UsingSystemComponentModelDataAnnotations}

{GeneratorOutput.GeneratorAtcComment}
namespace {ProjectPrefix}.{ProjectSuffix}.Contracts.{FocusOnSecment}
{{
    /// <summary>
    /// A string component.
    /// </summary>
    {GeneratorOutput.GeneratorClassAttribute}
    public class StringComponent
    {{
        /// <summary>
        /// normal string.
        /// </summary>
        public string StringProperty {{ get; set; }}

        /// <summary>
        /// uri string.
        /// </summary>
        /// <remarks>
        /// Url validation being enforced.
        /// </remarks>
        [Uri]
        public Uri UriProperty {{ get; set; }}

        /// <summary>
        /// uuid string.
        /// </summary>
        public Guid UuidProperty {{ get; set; }}

        /// <summary>
        /// Converts to string.
        /// </summary>
        public override string ToString()
        {{
            return $""{{nameof(StringProperty)}}: {{StringProperty}}, {{nameof(UriProperty)}}: {{UriProperty}}, {{nameof(UuidProperty)}}: {{UuidProperty}}"";
        }}
    }}
}}
".Trim();

        [Fact]
        public void TestSpecModelGenerator()
        {
            // Setup
            var apiProj = GeneratorTestSetup.CreateApiProject(spec, "TestProject", "AtcTest");

            var apiSchema = apiProj.Document.Components.Schemas.First().Value;

            var sut = new SyntaxGeneratorContractModel(apiProj, apiSchema.GetModelName(), apiSchema, FocusOnSecment);

            // Act
            var specOutput = sut.ToCodeAsString();

            // Assert
            Assert.Equal(expectedCode, specOutput);
        }
    }
}
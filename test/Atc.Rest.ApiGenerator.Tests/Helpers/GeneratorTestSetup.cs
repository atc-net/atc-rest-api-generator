using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Atc.Rest.ApiGenerator.Tests.Helpers
{
    public static class GeneratorTestSetup
    {
        public static ApiProjectOptions CreateApiProject(string spec, string projectPrefix, string projectSuffix)
        {
            var document = GenerateApiDocument(spec);

            return new ApiProjectOptions(
                new DirectoryInfo("resources"),
                null,
                document,
                new FileInfo("resources/dummySpec.yaml"),
                projectPrefix,
                projectSuffix,
                new Models.ApiOptions.ApiOptions(),
                null);
        }

        private static OpenApiDocument GenerateApiDocument(string spec)
        {
            using var specStream = GenerateStreamFromString(spec);
            var openApiStreamReader = new OpenApiStreamReader();
            var openApiDocument = openApiStreamReader.Read(specStream, out var diagnostic);
            return openApiDocument;
        }

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}

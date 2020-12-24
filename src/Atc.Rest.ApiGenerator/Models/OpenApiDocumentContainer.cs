using System;
using System.IO;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

namespace Atc.Rest.ApiGenerator.Models
{
    internal class OpenApiDocumentContainer
    {
        public OpenApiDocumentContainer(FileInfo docFile, string docText, OpenApiDocument openApiDocument, OpenApiDiagnostic openApiDiagnostic)
        {
            DocFile = docFile;
            DocText = docText;
            Document = openApiDocument;
            Diagnostic = openApiDiagnostic;
        }

        public OpenApiDocumentContainer(FileInfo docFile, string docText, Exception exception)
        {
            DocFile = docFile;
            DocText = docText;
            Exception = exception;
        }

        public FileInfo DocFile { get; }

        public string DocText { get; }

        public OpenApiDocument? Document { get; }

        public OpenApiDiagnostic? Diagnostic { get; }

        public Exception? Exception { get; }

        public override string ToString()
        {
            return Exception == null
                ? $"{nameof(DocFile)}: {DocFile}, ErrorCount: {Diagnostic?.Errors.Count}"
                : $"{nameof(DocFile)}: {DocFile}, Exception: {Exception.Message}";
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models;
using Atc.Rest.ApiGenerator.Models.ApiOptions;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable InvertIf
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ReturnTypeCanBeEnumerable.Local
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class OpenApiDocumentHelper
    {
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "OK.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
        public static Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> CombineAndGetApiDocument(string specificationPath)
        {
            if (specificationPath == null)
            {
                throw new ArgumentNullException(nameof(specificationPath));
            }

            FileInfo? apiDocFile;
            if (specificationPath.EndsWith(".yaml", StringComparison.Ordinal))
            {
                apiDocFile = specificationPath.StartsWith("http", StringComparison.CurrentCultureIgnoreCase)
                    ? HttpClientHelper.DownloadToTempFile(specificationPath)
                    : new FileInfo(specificationPath);

                if (apiDocFile == null || !apiDocFile.Exists)
                {
                    throw new IOException("Api yaml file don't exist.");
                }
            }
            else if (specificationPath.EndsWith(".json", StringComparison.Ordinal))
            {
                apiDocFile = specificationPath.StartsWith("http", StringComparison.CurrentCultureIgnoreCase)
                    ? HttpClientHelper.DownloadToTempFile(specificationPath)
                    : new FileInfo(specificationPath);

                if (apiDocFile == null || !apiDocFile.Exists)
                {
                    throw new IOException("Api json file don't exist.");
                }
            }
            else
            {
                // Find all yaml files, except files starting with '.' as example '.spectral.yaml'
                var docFiles = Directory
                    .GetFiles(specificationPath, "*.yaml", SearchOption.AllDirectories)
                    .Where(x => !x.Contains("\\.", StringComparison.Ordinal))
                    .ToArray();

                if (docFiles.Length == 0)
                {
                    // No yaml, then try all json files
                    docFiles = Directory
                        .GetFiles(specificationPath, "*.json", SearchOption.AllDirectories)
                        .Where(x => !x.Contains("\\.", StringComparison.Ordinal))
                        .ToArray();
                }

                apiDocFile = docFiles.Length switch
                {
                    0 => throw new IOException("Api specification file don't exist in folder."),
                    1 => new FileInfo(docFiles.First()),
                    _ => CreateCombineApiDocumentFile(specificationPath)
                };
            }

            if (apiDocFile == null || !apiDocFile.Exists)
            {
                throw new IOException("Api specification file don't exist");
            }

            var openApiStreamReader = new OpenApiStreamReader();
            var openApiDocument = openApiStreamReader.Read(File.OpenRead(apiDocFile.FullName), out var diagnostic);
            return new Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo>(openApiDocument, diagnostic, new FileInfo(apiDocFile.FullName));
        }

        public static List<LogKeyValueItem> Validate(Tuple<OpenApiDocument, OpenApiDiagnostic, FileInfo> apiDocument, ApiOptionsValidation validationOptions)
        {
            if (apiDocument == null)
            {
                throw new ArgumentNullException(nameof(apiDocument));
            }

            if (validationOptions == null)
            {
                throw new ArgumentNullException(nameof(validationOptions));
            }

            var logItems = new List<LogKeyValueItem>();
            if (apiDocument.Item2.SpecificationVersion == OpenApiSpecVersion.OpenApi2_0)
            {
                logItems.Add(LogItemHelper.Create(LogCategoryType.Error, "#", "OpenApi 2.x is not supported."));
                return logItems;
            }

            foreach (var diagnosticError in apiDocument.Item2.Errors)
            {
                if (diagnosticError.Message.EndsWith("#/components/schemas", StringComparison.Ordinal))
                {
                    continue;
                }

                var description = string.IsNullOrEmpty(diagnosticError.Pointer)
                    ? $"{diagnosticError.Message}"
                    : $"{diagnosticError.Message} <#> {diagnosticError.Pointer}";
                logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.OpenApiCore, description));
            }

            logItems.AddRange(OpenApiDocumentValidationHelper.ValidateDocument(apiDocument.Item1, validationOptions));
            return logItems;
        }

        public static List<string> GetBasePathSegmentNames(OpenApiDocument openApiDocument)
        {
            if (openApiDocument == null)
            {
                throw new ArgumentNullException(nameof(openApiDocument));
            }

            var names = new List<string>();
            if (openApiDocument.Paths?.Keys == null)
            {
                return names.ToList();
            }

            foreach (var name in openApiDocument.Paths.Keys
                    .Select(x => x.Split('/', StringSplitOptions.RemoveEmptyEntries))
                    .Where(sa => sa.Length != 0)
                    .Select(sa => sa[0].ToLower(CultureInfo.CurrentCulture)).Where(name => !names.Contains(name)))
            {
                names.Add(name);
            }

            return names
                .Select(x => x.PascalCase(true))
                .OrderBy(x => x)
                .ToList()!;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "OK.")]
        [SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "OK.")]
        private static List<OpenApiDocumentContainer> GetAllApiDocuments(DirectoryInfo specificationPath)
        {
            if (specificationPath == null)
            {
                throw new ArgumentNullException(nameof(specificationPath));
            }

            var result = new List<OpenApiDocumentContainer>();
            var docFiles = Directory.GetFiles(specificationPath.FullName, "*.yaml", SearchOption.AllDirectories);
            if (docFiles.Length == 0)
            {
                docFiles = Directory.GetFiles(specificationPath.FullName, "*.json", SearchOption.AllDirectories);
            }

            foreach (var docFile in docFiles)
            {
                var text = File.ReadAllText(docFile);
                try
                {
                    var openApiStreamReader = new OpenApiStreamReader();
                    var document = openApiStreamReader.Read(File.OpenRead(docFile), out var diagnostic);
                    result.Add(
                        new OpenApiDocumentContainer(
                            new FileInfo(docFile),
                            text,
                            document,
                            diagnostic));
                }
                catch (Exception ex)
                {
                    result.Add(
                        new OpenApiDocumentContainer(
                            new FileInfo(docFile),
                            text,
                            ex));
                }
            }

            return result;
        }

        [SuppressMessage("Info Code Smell", "S1135:Track uses of \"TODO\" tags", Justification = "Allow TODO here.")]
        [SuppressMessage("Minor Code Smell", "S1481:Unused local variables should be removed", Justification = "OK for now.")]
        private static FileInfo? CreateCombineApiDocumentFile(string specificationPath)
        {
            var openApiDocs = GetAllApiDocuments(new DirectoryInfo(specificationPath));

            // TODO: Combine all yaml files into 1
            throw new NotImplementedException();
        }
    }
}
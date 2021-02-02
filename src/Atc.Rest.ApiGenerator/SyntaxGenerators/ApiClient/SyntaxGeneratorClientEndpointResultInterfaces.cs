using System;
using System.Collections.Generic;
using System.Linq;
using Atc.Rest.ApiGenerator.Models;
using Microsoft.OpenApi.Models;

namespace Atc.Rest.ApiGenerator.SyntaxGenerators.ApiClient
{
    // ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
    // ReSharper disable UseDeconstruction
    public class SyntaxGeneratorClientEndpointResultInterfaces
    {
        public SyntaxGeneratorClientEndpointResultInterfaces(
            ApiProjectOptions apiProjectOptions,
            List<ApiOperationSchemaMap> operationSchemaMappings,
            string focusOnSegmentName)
        {
            this.ApiProjectOptions = apiProjectOptions ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.OperationSchemaMappings = operationSchemaMappings ?? throw new ArgumentNullException(nameof(apiProjectOptions));
            this.FocusOnSegmentName = focusOnSegmentName ?? throw new ArgumentNullException(nameof(focusOnSegmentName));
        }

        public ApiProjectOptions ApiProjectOptions { get; }

        public List<ApiOperationSchemaMap> OperationSchemaMappings { get; }

        public string FocusOnSegmentName { get; }

        public List<SyntaxGeneratorClientEndpointResultInterface> GenerateSyntaxTrees()
        {
            var list = new List<SyntaxGeneratorClientEndpointResultInterface>();
            foreach (var urlPath in ApiProjectOptions.Document.Paths)
            {
                if (!urlPath.IsPathStartingSegmentName(FocusOnSegmentName))
                {
                    continue;
                }

                list.AddRange(
                    urlPath.Value.Operations
                        .Select(x => new SyntaxGeneratorClientEndpointResultInterface(
                            ApiProjectOptions,
                            OperationSchemaMappings,
                            x.Key,
                            x.Value,
                            FocusOnSegmentName,
                            urlPath.Key,
                            urlPath.Value.HasParameters() || x.Value.HasParametersOrRequestBody()))
                        .Where(item => item.GenerateCode()));
            }

            return list;
        }
    }
}
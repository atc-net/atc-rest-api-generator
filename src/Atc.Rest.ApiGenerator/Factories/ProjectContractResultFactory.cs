using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;

// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Factories
{
    internal static class ProjectContractResultFactory
    {
        public static string[] CreateUsingList(OpenApiResponses responses, bool useProblemDetailsAsDefaultResponseBody)
        {
            if (responses == null)
            {
                throw new ArgumentNullException(nameof(responses));
            }

            var list = new List<string>
            {
                "System",
                "System.CodeDom.Compiler",
                "System.Diagnostics.CodeAnalysis",
                "Microsoft.AspNetCore.Mvc",
            };

            if (responses.HasSchemaTypeOfArray())
            {
                list.Add("System.Collections.Generic");
            }

            if (useProblemDetailsAsDefaultResponseBody)
            {
                list.Add("System.Net");
                list.Add("Atc.Rest.Results");
            }
            else
            {
                if (responses.HasSchemaTypeOfHttpStatusCodeUsingSystemNet())
                {
                    list.Add("System.Net");
                }

                list.Add("Atc.Rest.Results");

                if (responses.HasSchemaTypeOfHttpStatusCodeUsingAspNetCoreHttp())
                {
                    list.Add("Microsoft.AspNetCore.Http");
                }
            }

            return list.ToArray();
        }
    }
}
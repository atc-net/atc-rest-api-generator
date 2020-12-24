using System;
using System.Collections.Generic;
using Atc.Rest.ApiGenerator.Models;

namespace Atc.Rest.ApiGenerator.Factories
{
    internal static class ProjectHandlerFactory
    {
        public static string[] CreateUsingList(
            DomainProjectOptions domainProjectOptions,
            string focusOnSegmentName)
        {
            var list = new List<string>
            {
                "System.Threading",
                "System.Threading.Tasks",
                $"{domainProjectOptions.ProjectName.Replace(".Domain", ".Api", StringComparison.Ordinal)}.Generated.{NameConstants.Contracts}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}",
            };

            return list.ToArray();
        }
    }
}
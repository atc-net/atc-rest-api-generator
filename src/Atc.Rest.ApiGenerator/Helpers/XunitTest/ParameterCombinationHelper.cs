using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Helpers.XunitTest
{
    public static class ParameterCombinationHelper
    {
        public static List<List<OpenApiParameter>> GetCombination(List<OpenApiParameter> parameters, bool useForBadRequest)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var list = new List<List<OpenApiParameter>>();
            if (parameters.Count <= 0)
            {
                return list;
            }

            var totalNames = GetNames(parameters);
            var totalCombinationNames = totalNames.GetUniqueCombinations();
            var requiredNames = GetRequiredNames(parameters);
            if (requiredNames.Count > 0)
            {
                if (useForBadRequest)
                {
                    var requiredCombinationNames = requiredNames.GetUniqueCombinations();
                    list.AddRange(GetCombinationParameters(parameters, requiredCombinationNames));
                }
                else
                {
                    var filteredCombinationNames = GetFilteredCombinationNames(totalCombinationNames, requiredNames);
                    list.AddRange(GetCombinationParameters(parameters, filteredCombinationNames));
                }
            }
            else
            {
                if (!useForBadRequest)
                {
                    list.AddRange(GetCombinationParameters(parameters, totalCombinationNames));
                }
            }

            return list;
        }

        private static List<List<OpenApiParameter>> GetCombinationParameters(List<OpenApiParameter> parameters, IEnumerable<IEnumerable<string>> combinationNames)
        {
            var list = new List<List<OpenApiParameter>>();
            foreach (var item in combinationNames)
            {
                var localParameters = item
                    .Select(name => parameters.Find(x => x.Name.Equals(name, StringComparison.Ordinal)))
                    .ToList();
                list.Add(localParameters);
            }

            return list;
        }

        private static List<List<string>> GetFilteredCombinationNames(IEnumerable<IEnumerable<string>> combinationNames, List<string> requiredNames)
        {
            return combinationNames
                .Select(x => x.ToList())
                .Where(x => requiredNames.All(x.Contains))
                .ToList();
        }

        private static List<string> GetNames(List<OpenApiParameter> parameters)
        {
            return parameters.Select(x => x.Name).ToList();
        }

        private static List<string> GetRequiredNames(List<OpenApiParameter> parameters)
        {
            return parameters.Where(x => x.Required).Select(x => x.Name).ToList();
        }
    }
}
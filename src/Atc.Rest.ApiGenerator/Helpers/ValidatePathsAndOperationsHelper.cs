using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models.ApiOptions;
using Microsoft.OpenApi.Models;

// ReSharper disable ReturnTypeCanBeEnumerable.Global
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class ValidatePathsAndOperationsHelper
    {
        /// <summary>
        /// Check global parameters.
        /// </summary>
        /// <param name="validationOptions">The validation options.</param>
        /// <param name="globalPathParameterNames">The global path parameter names.</param>
        /// <param name="path">The path.</param>
        public static List<LogKeyValueItem> ValidateGlobalParameters(
            ApiOptionsValidation validationOptions,
            IEnumerable<string> globalPathParameterNames,
            KeyValuePair<string, OpenApiPathItem> path)
        {
            if (validationOptions == null)
            {
                throw new ArgumentNullException(nameof(validationOptions));
            }

            if (globalPathParameterNames == null)
            {
                throw new ArgumentNullException(nameof(globalPathParameterNames));
            }

            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            foreach (var pathParameterName in globalPathParameterNames)
            {
                if (!path.Key.Contains(pathParameterName, StringComparison.OrdinalIgnoreCase))
                {
                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation11, $"Defined global path parameter '{pathParameterName}' does not exist in route '{path.Key}'."));
                }
            }

            return logItems;
        }

        /// <summary>
        /// Check for operations that are not defining parameters, which are present in the path.key.
        /// </summary>
        /// <param name="validationOptions">The validation options.</param>
        /// <param name="path">The path.</param>
        public static List<LogKeyValueItem> ValidateMissingOperationParameters(
            ApiOptionsValidation validationOptions,
            KeyValuePair<string, OpenApiPathItem> path)
        {
            if (validationOptions == null)
            {
                throw new ArgumentNullException(nameof(validationOptions));
            }

            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            if (!path.Key.Contains('{', StringComparison.Ordinal) ||
                                               !path.Key.IsStringFormatParametersBalanced(false))
            {
                return logItems;
            }

            var parameterNamesToCheckAgainst = GetParameterListFromPathKey(path.Key);
            var allOperationsParametersFromPath = GetAllOperationsParametersFromPath(path.Value.Operations);
            var distinctOperations = allOperationsParametersFromPath
                .Select(x => x.Item1)
                .Distinct()
                .ToList();

            foreach (var parameterName in parameterNamesToCheckAgainst)
            {
                var allOperationWithTheMatchingParameterName = allOperationsParametersFromPath
                    .Where(x => x.Item2.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (distinctOperations.Count != allOperationWithTheMatchingParameterName.Count)
                {
                    var operationsWithMissingParameter = allOperationsParametersFromPath
                        .Where(x => string.IsNullOrEmpty(x.Item2))
                        .Select(x => x.Item1)
                        .ToList();

                    logItems.Add(operationsWithMissingParameter.Count == 0
                        ? LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation12, $"The operations in path '{path.Key}' does not define a parameter named '{parameterName}'.")
                        : LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation12, $"The operations '{string.Join(',', operationsWithMissingParameter)}' in path '{path.Key}' does not define a parameter named '{parameterName}'."));
                }
            }

            return logItems;
        }

        /// <summary>
        /// Check for operations with parameters, that are not present in the path.key.
        /// </summary>
        /// <param name="validationOptions">The validation options.</param>
        /// <param name="path">The path.</param>
        public static List<LogKeyValueItem> ValidateOperationsWithParametersNotPresentInPath(
            ApiOptionsValidation validationOptions,
            KeyValuePair<string, OpenApiPathItem> path)
        {
            if (validationOptions == null)
            {
                throw new ArgumentNullException(nameof(validationOptions));
            }

            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            var openApiOperationsWithPathParameter = path.Value.Operations.Values
                .Where(x => x.Parameters.Any(p => p.In == ParameterLocation.Path))
                .ToList();

            if (!openApiOperationsWithPathParameter.Any())
            {
                return logItems;
            }

            var operationPathParameterNames = new List<string>();
            foreach (var openApiOperation in openApiOperationsWithPathParameter)
            {
                operationPathParameterNames.AddRange(openApiOperation.Parameters
                    .Where(x => x.In == ParameterLocation.Path)
                    .Select(openApiParameter => openApiParameter.Name));
            }

            if (!operationPathParameterNames.Any())
            {
                return logItems;
            }

            foreach (var operationParameterName in operationPathParameterNames)
            {
                if (!path.Key.Contains(operationParameterName, StringComparison.OrdinalIgnoreCase))
                {
                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation13, $"Defined path parameter '{operationParameterName}' does not exist in route '{path.Key}'."));
                }
            }

            return logItems;
        }

        /// <summary>
        /// Check for response types according to operation/global parameters.
        /// </summary>
        /// <param name="validationOptions">The validation options.</param>
        /// <param name="path">The path.</param>
        public static List<LogKeyValueItem> ValidateGetOperations(
            ApiOptionsValidation validationOptions,
            KeyValuePair<string, OpenApiPathItem> path)
        {
            if (validationOptions == null)
            {
                throw new ArgumentNullException(nameof(validationOptions));
            }

            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            foreach (var (key, value) in path.Value.Operations)
            {
                if (key != OperationType.Get || (path.Value.Parameters.All(x => x.In != ParameterLocation.Path) &&
                                                 value.Parameters.All(x => x.In != ParameterLocation.Path)))
                {
                    continue;
                }

                var httpStatusCodes = value.Responses.GetHttpStatusCodes();
                if (!httpStatusCodes.Contains(HttpStatusCode.NotFound))
                {
                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation14, $"Missing NotFound response type for operation '{value.GetOperationName()}', required by url parameter."));
                }
            }

            return logItems;
        }

        private static IEnumerable<string> GetParameterListFromPathKey(string pathKey)
        {
            var sa = pathKey
                .Split('{', StringSplitOptions.RemoveEmptyEntries)
                .Where(x => x.Contains('}', StringComparison.Ordinal))
                .ToList();

            return sa
                .Select(item => item.Substring(0, item.IndexOf('}', StringComparison.Ordinal)))
                .ToList();
        }

        private static List<Tuple<string, string>> GetAllOperationsParametersFromPath(IDictionary<OperationType, OpenApiOperation> apiOperations)
        {
            var list = new List<Tuple<string, string>>();
            foreach (var apiOperation in apiOperations.Values)
            {
                if (apiOperation.Parameters.Any())
                {
                    foreach (var apiOperationParameter in apiOperation.Parameters)
                    {
                        list.Add(Tuple.Create(apiOperation.GetOperationName(), apiOperationParameter.Name));
                    }
                }
                else
                {
                    list.Add(Tuple.Create(apiOperation.GetOperationName(), string.Empty));
                }
            }

            return list;
        }
    }
}
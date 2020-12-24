using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using Atc.Data.Models;
using Atc.Rest.ApiGenerator.Models.ApiOptions;
using Microsoft.OpenApi.Models;

// ReSharper disable SwitchStatementMissingSomeEnumCasesNoDefault
// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault
// ReSharper disable LocalizableElement
// ReSharper disable ReturnTypeCanBeEnumerable.Local
// ReSharper disable InvertIf
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class OpenApiDocumentValidationHelper
    {
        public static List<LogKeyValueItem> ValidateDocument(OpenApiDocument apiDocument, ApiOptionsValidation validationOptions)
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
            logItems.AddRange(ValidateSchemas(validationOptions, apiDocument.Components.Schemas.Values));
            logItems.AddRange(ValidateOperations(validationOptions, apiDocument.Paths, apiDocument.Components.Schemas));
            logItems.AddRange(ValidatePathsAndOperations(validationOptions, apiDocument.Paths));
            logItems.AddRange(ValidateOperationsParametersAndResponses(validationOptions, apiDocument.Paths.Values));

            return logItems;
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private static List<LogKeyValueItem> ValidateSchemas(
            ApiOptionsValidation validationOptions,
            ICollection<OpenApiSchema> schemas)
        {
            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            foreach (var schema in schemas)
            {
                switch (schema.Type)
                {
                    case OpenApiDataTypeConstants.Array:
                        {
                            if (string.IsNullOrEmpty(schema.Title))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema01, $"Missing title on array type '{schema.Reference.ReferenceV3}'."));
                            }
                            else if (schema.Title.IsFirstCharacterLowerCase())
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema02, $"Title on array type '{schema.Title}' is not starting with uppercase."));
                            }

                            logItems.AddRange(ValidateSchemaModelNameCasing(validationOptions, schema));
                            break;
                        }

                    case OpenApiDataTypeConstants.Object:
                        {
                            if (string.IsNullOrEmpty(schema.Title))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema03, $"Missing title on object type '{schema.Reference.ReferenceV3}'."));
                            }
                            else if (schema.Title.IsFirstCharacterLowerCase())
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema04, $"Title on object type '{schema.Title}' is not starting with uppercase."));
                            }

                            foreach (var (key, value) in schema.Properties)
                            {
                                if (value.Nullable && schema.Required.Contains(key))
                                {
                                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema08, $"Nullable property '{key}' must not be present in required property list in type '{schema.Reference.ReferenceV3}'."));
                                }

                                switch (value.Type)
                                {
                                    case OpenApiDataTypeConstants.Object:
                                        {
                                            if (!value.IsObjectReferenceTypeDeclared())
                                            {
                                                logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.Schema10, $"Implicit object definition on property '{key}' in type '{schema.Reference.ReferenceV3}' is not supported."));
                                            }

                                            break;
                                        }

                                    case OpenApiDataTypeConstants.Array:
                                        {
                                            if (value.Items == null)
                                            {
                                                logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.Schema11, $"Not specifying a data type for array property '{key}' in type '{schema.Reference.ReferenceV3}' is not supported."));
                                            }
                                            else
                                            {
                                                if (value.Items.Type == null)
                                                {
                                                    logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.Schema09, $"Not specifying a data type for array property '{key}' in type '{schema.Reference.ReferenceV3}' is not supported."));
                                                }

                                                if (value.Items.Type != null && !value.IsArrayReferenceTypeDeclared() && !value.IsItemsOfSimpleDataType())
                                                {
                                                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema05, $"Implicit object definition on property '{key}' in array type '{schema.Reference.ReferenceV3}' is not supported."));
                                                }
                                            }

                                            break;
                                        }
                                }

                                logItems.AddRange(ValidateSchemaModelPropertyNameCasing(validationOptions, key, schema));
                            }

                            logItems.AddRange(ValidateSchemaModelNameCasing(validationOptions, schema));
                            break;
                        }
                }
            }

            return logItems;
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private static List<LogKeyValueItem> ValidateOperations(
            ApiOptionsValidation validationOptions,
            OpenApiPaths paths,
            IDictionary<string, OpenApiSchema> modelSchemas)
        {
            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            foreach (var (pathKey, pathValue) in paths)
            {
                foreach (var (operationKey, operationValue) in pathValue.Operations)
                {
                    if (string.IsNullOrEmpty(operationValue.OperationId))
                    {
                        logItems.Add(LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.Operation01, $"Missing OperationId in path '{operationKey} # {pathKey}'."));
                    }
                    else
                    {
                        if (!operationValue.OperationId.IsCasingStyleValid(validationOptions.OperationIdCasingStyle))
                        {
                            logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation02, $"OperationId '{operationValue.OperationId}' is not using {validationOptions.OperationIdCasingStyle}."));
                        }

                        if (operationKey == OperationType.Get)
                        {
                            if (!operationValue.OperationId.StartsWith("Get", StringComparison.OrdinalIgnoreCase) &&
                                !operationValue.OperationId.StartsWith("List", StringComparison.OrdinalIgnoreCase))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation03, $"OperationId should start with the prefix 'Get' or 'List' for operation '{operationValue.GetOperationName()}'."));
                            }
                        }
                        else if (operationKey == OperationType.Post)
                        {
                            if (operationValue.OperationId.StartsWith("Delete", StringComparison.OrdinalIgnoreCase))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation04, $"OperationId should not start with the prefix 'Delete' for operation '{operationValue.GetOperationName()}'."));
                            }
                        }
                        else if (operationKey == OperationType.Put)
                        {
                            if (!operationValue.OperationId.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation05, $"OperationId should start with the prefix 'Update' for operation '{operationValue.GetOperationName()}'."));
                            }
                        }
                        else if (operationKey == OperationType.Patch)
                        {
                            if (!operationValue.OperationId.StartsWith("Patch", StringComparison.OrdinalIgnoreCase) &&
                                !operationValue.OperationId.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation06, $"OperationId should start with the prefix 'Update' for operation '{operationValue.GetOperationName()}'."));
                            }
                        }
                        else if (operationKey == OperationType.Delete &&
                                 !operationValue.OperationId.StartsWith("Delete", StringComparison.OrdinalIgnoreCase) &&
                                 !operationValue.OperationId.StartsWith("Remove", StringComparison.OrdinalIgnoreCase))
                        {
                            logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation07, $"OperationId should start with the prefix 'Delete' for operation '{operationValue.GetOperationName()}'."));
                        }
                    }
                }

                foreach (var (operationKey, operationValue) in pathValue.Operations)
                {
                    // Validate Response Schema
                    var responseModelSchema = operationValue.GetModelSchemaFromResponse();
                    if (responseModelSchema != null)
                    {
                        if (operationValue.IsOperationIdPluralized(operationKey))
                        {
                            if (!IsModelOfTypeArray(responseModelSchema, modelSchemas))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation08, $"OperationId '{operationValue.GetOperationName()}' is not singular - Response model is defined as a single item."));
                            }
                        }
                        else
                        {
                            if (IsModelOfTypeArray(responseModelSchema, modelSchemas))
                            {
                                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation09, $"OperationId '{operationValue.GetOperationName()}' is not pluralized - Response model is defined as an array."));
                            }
                        }
                    }

                    //// TO-DO Validate RequestBody Schema
                }
            }

            return logItems;
        }

        private static List<LogKeyValueItem> ValidatePathsAndOperations(
            ApiOptionsValidation validationOptions,
            OpenApiPaths paths)
        {
            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            foreach (var path in paths)
            {
                if (!path.Key.IsStringFormatParametersBalanced(false))
                {
                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Path01, $"Path parameters are not well-formatted for '{path.Key}'."));
                }

                var globalPathParameterNames = path.Value.Parameters
                    .Where(x => x.In == ParameterLocation.Path)
                    .Select(x => x.Name)
                    .ToList();

                if (globalPathParameterNames.Any())
                {
                    logItems.AddRange(ValidatePathsAndOperationsHelper.ValidateGlobalParameters(validationOptions, globalPathParameterNames, path));
                }
                else
                {
                    logItems.AddRange(ValidatePathsAndOperationsHelper.ValidateMissingOperationParameters(validationOptions, path));
                    logItems.AddRange(ValidatePathsAndOperationsHelper.ValidateOperationsWithParametersNotPresentInPath(validationOptions, path));
                }

                logItems.AddRange(ValidatePathsAndOperationsHelper.ValidateGetOperations(validationOptions, path));
            }

            return logItems;
        }

        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "OK.")]
        private static List<LogKeyValueItem> ValidateOperationsParametersAndResponses(
            ApiOptionsValidation validationOptions,
            Dictionary<string, OpenApiPathItem>.ValueCollection paths)
        {
            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            foreach (var path in paths)
            {
                foreach (var (_, value) in path.Operations)
                {
                    var httpStatusCodes = value.Responses.GetHttpStatusCodes();
                    if (httpStatusCodes.Contains(HttpStatusCode.BadRequest) &&
                        !value.HasParametersOrRequestBody() && !path.HasParameters())
                    {
                        logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation10, $"Contains BadRequest response type for operation '{value.GetOperationName()}', but has no parameters."));
                    }

                    if (httpStatusCodes.Contains(HttpStatusCode.OK) && httpStatusCodes.Contains(HttpStatusCode.Created))
                    {
                        // We do not support both 200 and 201, since our ActionResult - implicit operators only supports 1 type.
                        logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation18, $"The operation '{value.GetOperationName()}' contains both 200 and 201, which is not supported."));
                    }

                    if (value.HasParametersOrRequestBody())
                    {
                        var schema = value.RequestBody?.Content.GetSchema();
                        if (schema != null && string.IsNullOrEmpty(schema.GetModelName()))
                        {
                            logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation17, $"RequestBody is defined without model for operation '{value.GetOperationName()}'."));
                        }
                    }

                    foreach (var parameter in value.Parameters)
                    {
                        switch (parameter.In)
                        {
                            case ParameterLocation.Path:
                                if (!parameter.Required)
                                {
                                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation15, $"Path parameter '{parameter.Name}' for operation '{value.GetOperationName()}' is missing required=true."));
                                }

                                if (parameter.Schema.Nullable)
                                {
                                    logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Operation16, $"Path parameter '{parameter.Name}' for operation '{value.GetOperationName()}' must not be nullable."));
                                }

                                break;
                            case ParameterLocation.Query:
                                break;
                        }
                    }
                }
            }

            return logItems;
        }

        private static List<LogKeyValueItem> ValidateSchemaModelNameCasing(
            ApiOptionsValidation validationOptions,
            OpenApiSchema schema)
        {
            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            var modelName = schema.GetModelName(false);
            if (!modelName.IsCasingStyleValid(validationOptions.ModelNameCasingStyle))
            {
                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema06, $"Object '{modelName}' is not using {validationOptions.ModelNameCasingStyle}."));
            }

            return logItems;
        }

        private static List<LogKeyValueItem> ValidateSchemaModelPropertyNameCasing(
            ApiOptionsValidation validationOptions,
            string key,
            OpenApiSchema schema)
        {
            var logItems = new List<LogKeyValueItem>();
            var logCategory = validationOptions.StrictMode
                ? LogCategoryType.Error
                : LogCategoryType.Warning;

            if (!key.IsCasingStyleValid(validationOptions.ModelPropertyNameCasingStyle))
            {
                logItems.Add(LogItemHelper.Create(logCategory, ValidationRuleNameConstants.Schema07, $"Object '{schema.Title}' with property '{key}' is not using {validationOptions.ModelPropertyNameCasingStyle}."));
            }

            return logItems;
        }

        private static bool IsModelOfTypeArray(OpenApiSchema schema, IDictionary<string, OpenApiSchema> modelSchemas)
        {
            var modelType = schema.GetModelType();
            if (modelType == null && schema.Reference?.Id != null)
            {
                var (key, value) = modelSchemas.FirstOrDefault(x => x.Key.Equals(schema.Reference.Id, StringComparison.OrdinalIgnoreCase));
                if (key != null)
                {
                    return value.Type != null && value.Type.EndsWith(OpenApiDataTypeConstants.Array, StringComparison.OrdinalIgnoreCase);
                }
            }

            return modelType != null && modelType.EndsWith(OpenApiDataTypeConstants.Array, StringComparison.OrdinalIgnoreCase);
        }
    }
}
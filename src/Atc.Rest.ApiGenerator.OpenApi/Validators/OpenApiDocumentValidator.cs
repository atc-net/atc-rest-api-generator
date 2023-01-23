// ReSharper disable InvertIf
// ReSharper disable ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.OpenApi.Validators;

public class OpenApiDocumentValidator : IOpenApiDocumentValidator
{
    private const string AreaValidationEmoji = "🔍";

    private readonly ILogger<OpenApiDocumentValidator> logger;
    private readonly ILogItemFactory logItemFactory;
    private readonly List<LogKeyValueItem> logItems = new();

    private LogCategoryType logCategory = LogCategoryType.Error;

    public OpenApiDocumentValidator(
        ILogger<OpenApiDocumentValidator> logger,
        ILogItemFactory logItemFactory)
    {
        this.logger = logger;
        this.logItemFactory = logItemFactory;
    }

    public bool IsValid(
        ApiOptionsValidation apiOptionsValidation,
        OpenApiDocumentContainer apiDocumentContainer)
    {
        logger.LogInformation($"{AreaValidationEmoji} Working on validation");

        logItems.Clear();
        logCategory = apiOptionsValidation.StrictMode
            ? LogCategoryType.Error
            : LogCategoryType.Warning;

        if (apiDocumentContainer.Document is null)
        {
            return false;
        }

        return IsValidUsingMicrosoftOpenApi(apiDocumentContainer) &&
               IsValidUsingAtcOptions(apiOptionsValidation, apiDocumentContainer);
    }

    private bool IsValidUsingMicrosoftOpenApi(
        OpenApiDocumentContainer apiDocumentContainer)
    {
        if (apiDocumentContainer.Diagnostic!.Errors.Count > 0)
        {
            var validationErrors = apiDocumentContainer.Diagnostic.Errors
                .Where(e => !e.Message.EndsWith(
                    "#/components/schemas",
                    StringComparison.Ordinal))
                .Select(e => logItemFactory.Create(
                    LogCategoryType.Error,
                    ValidationRuleNameConstants.OpenApiCore,
                    string.IsNullOrEmpty(e.Pointer)
                        ? $"{e.Message}"
                        : $"{e.Message} <#> {e.Pointer}"))
                .ToList();

            logger.LogKeyValueItems(validationErrors);
            return false;
        }

        if (apiDocumentContainer.Diagnostic.SpecificationVersion == OpenApiSpecVersion.OpenApi2_0)
        {
            logger.LogError("OpenApi 2.x is not supported.");
            return false;
        }

        return true;
    }

    private bool IsValidUsingAtcOptions(
        ApiOptionsValidation validationOptions,
        OpenApiDocumentContainer apiDocumentContainer)
    {
        ValidateSecurity(apiDocumentContainer.Document!);
        ValidateServers(apiDocumentContainer.Document!.Servers);
        ValidateSchemas(validationOptions, apiDocumentContainer.Document!.Components.Schemas.Values);
        ValidateOperations(validationOptions, apiDocumentContainer.Document.Paths, apiDocumentContainer.Document!.Components.Schemas);
        ValidatePathsAndOperations(apiDocumentContainer.Document!.Paths);
        ValidateOperationsParametersAndResponses(apiDocumentContainer.Document!.Paths.Values);

        AddIndentationToLogItemKeys(logItems);

        logger.LogKeyValueItems(logItems);
        return logItems.All(x => x.LogCategory != LogCategoryType.Error);
    }

    private void ValidateSecurity(
        OpenApiDocument apiDocument)
    {
        var globalAuthorizeRoles = new List<string>();
        var globalAuthenticationSchemes = new List<string>();

        if (apiDocument.Extensions.Any())
        {
            globalAuthorizeRoles.AddRange(apiDocument.Extensions.ExtractAuthorizationRoles());
            globalAuthenticationSchemes.AddRange(apiDocument.Extensions.ExtractAuthenticationSchemes());
        }

        foreach (var apiDocumentPath in apiDocument.Paths)
        {
            ValidatePathSecurity(apiDocumentPath, globalAuthorizeRoles, globalAuthenticationSchemes);

            foreach (var openApiOperation in apiDocumentPath.Value.Operations)
            {
                ValidateOperationSecurity(
                    openApiOperation,
                    globalAuthorizeRoles,
                    globalAuthenticationSchemes);
            }
        }
    }

    private void ValidatePathSecurity(
        KeyValuePair<string, OpenApiPathItem> apiDocumentPath,
        IReadOnlyCollection<string> globalAuthorizeRoles,
        IReadOnlyCollection<string> globalAuthenticationSchemes)
    {
        if (!apiDocumentPath.Value.Extensions.Any())
        {
            return;
        }

        var apiPathAuthenticationRequired = apiDocumentPath.Value.Extensions.ExtractAuthenticationRequired();
        var apiPathAuthorizeRoles = apiDocumentPath.Value.Extensions.ExtractAuthorizationRoles();
        var apiPathAuthenticationSchemes = apiDocumentPath.Value.Extensions.ExtractAuthenticationSchemes();

        if (apiPathAuthenticationRequired is not null &&
            !apiPathAuthenticationRequired.Value &&
            (apiPathAuthorizeRoles.Any() || apiPathAuthenticationSchemes.Any()))
        {
            logItems.Add(
                logItemFactory.Create(
                    logCategory,
                    ValidationRuleNameConstants.Security10,
                    $"Path '{apiDocumentPath.Key}' has {SecurityExtensionNameConstants.AuthenticationRequired} set to {apiPathAuthenticationRequired} but have {SecurityExtensionNameConstants.AuthorizeRoles} and/or {SecurityExtensionNameConstants.AuthenticationSchemes} set."));
        }

        var pathAuthorizeRoles = apiDocumentPath.Value.Extensions.ExtractAuthorizationRoles();
        if (pathAuthorizeRoles.Any())
        {
            foreach (var pathAuthorizeRole in pathAuthorizeRoles)
            {
                if (!globalAuthorizeRoles.Contains(pathAuthorizeRole, StringComparer.OrdinalIgnoreCase))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            LogCategoryType.Error,
                            ValidationRuleNameConstants.Security01,
                            $"Path '{apiDocumentPath.Key}' has the role '{pathAuthorizeRole}' defined which is not defined in the global {SecurityExtensionNameConstants.AuthorizeRoles} section."));
                }
                else if (globalAuthorizeRoles.Contains(pathAuthorizeRole, StringComparer.OrdinalIgnoreCase) &&
                         !globalAuthorizeRoles.Contains(pathAuthorizeRole, StringComparer.Ordinal))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            logCategory,
                            ValidationRuleNameConstants.Security08,
                            $"Path '{apiDocumentPath.Key}' has the role '{pathAuthorizeRole}' defined, but is using incorrect casing, when compared with role in global {SecurityExtensionNameConstants.AuthorizeRoles} section."));
                }
            }
        }

        var pathAuthenticationSchemes = apiDocumentPath.Value.Extensions.ExtractAuthenticationSchemes();
        if (pathAuthenticationSchemes.Any())
        {
            foreach (var pathAuthenticationScheme in pathAuthenticationSchemes)
            {
                if (!globalAuthenticationSchemes.Contains(pathAuthenticationScheme, StringComparer.OrdinalIgnoreCase))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            LogCategoryType.Error,
                            ValidationRuleNameConstants.Security02,
                            $"Path '{apiDocumentPath.Key}' has the authentication scheme '{pathAuthenticationScheme}' defined which is not defined in the global {SecurityExtensionNameConstants.AuthenticationSchemes} section."));
                }
                else if (globalAuthenticationSchemes.Contains(pathAuthenticationScheme, StringComparer.OrdinalIgnoreCase) &&
                         !globalAuthenticationSchemes.Contains(pathAuthenticationScheme, StringComparer.Ordinal))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            logCategory,
                            ValidationRuleNameConstants.Security09,
                            $"Path '{apiDocumentPath.Key}' has the authentication scheme '{pathAuthenticationScheme}' defined, but is using incorrect casing, when compared with authentication scheme in global {SecurityExtensionNameConstants.AuthenticationSchemes} section."));
                }
            }
        }
    }

    private void ValidateOperationSecurity(
        KeyValuePair<OperationType, OpenApiOperation> openApiOperation,
        IReadOnlyCollection<string> globalAuthorizeRoles,
        IReadOnlyCollection<string> globalAuthenticationSchemes)
    {
        if (!openApiOperation.Value.Extensions.Any())
        {
            return;
        }

        var operationName = openApiOperation.Value.OperationId.EnsureFirstCharacterToUpper();

        var apiOperationAuthenticationRequired = openApiOperation.Value.Extensions.ExtractAuthenticationRequired();
        var apiOperationAuthorizeRoles = openApiOperation.Value.Extensions.ExtractAuthorizationRoles();
        var apiOperationAuthenticationSchemes = openApiOperation.Value.Extensions.ExtractAuthenticationSchemes();

        if (apiOperationAuthenticationRequired is not null &&
            !apiOperationAuthenticationRequired.Value &&
            (apiOperationAuthorizeRoles.Any() || apiOperationAuthenticationSchemes.Any()))
        {
            logItems.Add(
                logItemFactory.Create(
                    logCategory,
                    ValidationRuleNameConstants.Security05,
                    $"Operation '{operationName}' has {SecurityExtensionNameConstants.AuthenticationRequired} set to {apiOperationAuthenticationRequired} but have {SecurityExtensionNameConstants.AuthorizeRoles} and/or {SecurityExtensionNameConstants.AuthenticationSchemes} set."));
        }

        if (apiOperationAuthorizeRoles.Any())
        {
            foreach (var pathItemAuthorizeRole in apiOperationAuthorizeRoles)
            {
                if (!globalAuthorizeRoles.Contains(pathItemAuthorizeRole, StringComparer.OrdinalIgnoreCase))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            LogCategoryType.Error,
                            ValidationRuleNameConstants.Security03,
                            $"Operation '{operationName}' has the role '{pathItemAuthorizeRole}' defined which is not defined in the global {SecurityExtensionNameConstants.AuthorizeRoles} section."));
                }
                else if (globalAuthorizeRoles.Contains(pathItemAuthorizeRole, StringComparer.OrdinalIgnoreCase) &&
                         !globalAuthorizeRoles.Contains(pathItemAuthorizeRole, StringComparer.Ordinal))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            logCategory,
                            ValidationRuleNameConstants.Security06,
                            $"Operation '{operationName}' has the role '{pathItemAuthorizeRole}' defined, but is using incorrect casing, when compared with role in global {SecurityExtensionNameConstants.AuthorizeRoles} section."));
                }
            }
        }

        if (apiOperationAuthenticationSchemes.Any())
        {
            foreach (var pathItemAuthenticationScheme in apiOperationAuthenticationSchemes)
            {
                if (!globalAuthenticationSchemes.Contains(pathItemAuthenticationScheme, StringComparer.OrdinalIgnoreCase))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            LogCategoryType.Error,
                            ValidationRuleNameConstants.Security04,
                            $"Operation '{operationName}' has the authentication scheme '{pathItemAuthenticationScheme}' defined which is not defined in the global {SecurityExtensionNameConstants.AuthenticationSchemes} section."));
                }
                else if (globalAuthenticationSchemes.Contains(pathItemAuthenticationScheme, StringComparer.OrdinalIgnoreCase) &&
                         !globalAuthenticationSchemes.Contains(pathItemAuthenticationScheme, StringComparer.Ordinal))
                {
                    logItems.Add(
                        logItemFactory.Create(
                            logCategory,
                            ValidationRuleNameConstants.Security07,
                            $"Operation '{operationName}' has the authentication scheme '{pathItemAuthenticationScheme}' defined, but is using incorrect casing, when compared with authentication scheme in global {SecurityExtensionNameConstants.AuthenticationSchemes} section."));
                }
            }
        }
    }

    private void ValidateServers(
        IEnumerable<OpenApiServer> servers)
    {
        var server = servers.FirstOrDefault();

        if (server is not null &&
            !IsServerUrlValid(server.Url))
        {
            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Server01, "Invalid server url."));
        }
    }

    private static bool IsServerUrlValid(
        string serverUrl)
    {
        if (string.IsNullOrWhiteSpace(serverUrl))
        {
            return false;
        }

        if (serverUrl.Equals("/", StringComparison.Ordinal))
        {
            return true;
        }

        if (serverUrl.EndsWith('/'))
        {
            return false;
        }

        var s = serverUrl
            .Replace("http://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase);
        if (s.Contains("//", StringComparison.Ordinal))
        {
            return false;
        }

        return serverUrl.StartsWith('/') ||
               serverUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
               serverUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
    }

    private void ValidateSchemas(
        ApiOptionsValidation validationOptions,
        IEnumerable<OpenApiSchema> schemas)
    {
        foreach (var schema in schemas)
        {
            switch (schema.Type)
            {
                case OpenApiDataTypeConstants.Array:
                {
                    if (string.IsNullOrEmpty(schema.Title))
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema01, $"Missing title on array type '{schema.Reference.ReferenceV3}'."));
                    }
                    else if (schema.Title.IsFirstCharacterLowerCase())
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema02, $"Title on array type '{schema.Title}' is not starting with uppercase."));
                    }

                    ValidateSchemaModelNameCasing(validationOptions, schema);
                    break;
                }

                case OpenApiDataTypeConstants.Object:
                {
                    if (string.IsNullOrEmpty(schema.Title))
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema03, $"Missing title on object type '{schema.Reference.ReferenceV3}'."));
                    }
                    else if (schema.Title.IsFirstCharacterLowerCase())
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema04, $"Title on object type '{schema.Title}' is not starting with uppercase."));
                    }

                    foreach (var (key, value) in schema.Properties)
                    {
                        if (string.IsNullOrEmpty(key))
                        {
                            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema12, $"Missing key/name for one or more properties on object type '{schema.Reference.ReferenceV3}'."));
                            break;
                        }

                        switch (value.Type)
                        {
                            case OpenApiDataTypeConstants.Object:
                            {
                                if (!value.IsObjectReferenceTypeDeclared())
                                {
                                    logItems.Add(logItemFactory.Create(LogCategoryType.Error, ValidationRuleNameConstants.Schema10, $"Implicit object definition on property '{key}' in type '{schema.Reference.ReferenceV3}' is not supported."));
                                }

                                break;
                            }

                            case OpenApiDataTypeConstants.Array:
                            {
                                if (value.Items is null)
                                {
                                    logItems.Add(logItemFactory.Create(LogCategoryType.Error, ValidationRuleNameConstants.Schema11, $"Not specifying a data type for array property '{key}' in type '{schema.Reference.ReferenceV3}' is not supported."));
                                }
                                else
                                {
                                    if (value.Items.Type is null &&
                                        !key.IsNamedAsItemsOrResult())
                                    {
                                        logItems.Add(logItemFactory.Create(LogCategoryType.Error, ValidationRuleNameConstants.Schema09, $"Not specifying a data type for array property '{key}' in type '{schema.Reference.ReferenceV3}' is not supported."));
                                    }

                                    if (value.Items.Type is not null &&
                                        !value.IsArrayReferenceTypeDeclared() &&
                                        !value.HasItemsWithSimpleDataType() &&
                                        !value.HasItemsWithFormatTypeBinary())
                                    {
                                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema05, $"Implicit object definition on property '{key}' in array type '{schema.Reference.ReferenceV3}' is not supported."));
                                    }
                                }

                                break;
                            }
                        }

                        ValidateSchemaModelPropertyNameCasing(validationOptions, key, schema);
                    }

                    ValidateSchemaModelNameCasing(validationOptions, schema);
                    break;
                }
            }
        }
    }

    private void ValidateSchemaModelNameCasing(
        ApiOptionsValidation validationOptions,
        OpenApiSchema schema)
    {
        var modelName = schema.GetModelName(ensureFirstCharacterToUpper: false);
        if (!modelName.IsCasingStyleValid(validationOptions.ModelNameCasingStyle))
        {
            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema06, $"Object '{modelName}' is not using {validationOptions.ModelNameCasingStyle}."));
        }
    }

    private void ValidateSchemaModelPropertyNameCasing(
        ApiOptionsValidation validationOptions,
        string key,
        OpenApiSchema schema)
    {
        if (!key.IsCasingStyleValid(validationOptions.ModelPropertyNameCasingStyle))
        {
            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Schema07, $"Object '{schema.Title}' with property '{key}' is not using {validationOptions.ModelPropertyNameCasingStyle}."));
        }
    }

    private void ValidateOperations(
        ApiOptionsValidation validationOptions,
        OpenApiPaths paths,
        IDictionary<string, OpenApiSchema> modelSchemas)
    {
        foreach (var (pathKey, pathValue) in paths)
        {
            foreach (var (operationKey, operationValue) in pathValue.Operations)
            {
                if (string.IsNullOrEmpty(operationValue.OperationId))
                {
                    logItems.Add(logItemFactory.Create(LogCategoryType.Error, ValidationRuleNameConstants.Operation01, $"Missing OperationId in path '{operationKey} # {pathKey}'."));
                }
                else
                {
                    if (!operationValue.OperationId.IsCasingStyleValid(validationOptions.OperationIdCasingStyle))
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation02, $"OperationId '{operationValue.OperationId}' is not using {validationOptions.OperationIdCasingStyle}."));
                    }

                    if (operationKey == OperationType.Get)
                    {
                        if (!operationValue.OperationId.StartsWith("Get", StringComparison.OrdinalIgnoreCase) &&
                            !operationValue.OperationId.StartsWith("List", StringComparison.OrdinalIgnoreCase))
                        {
                            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation03, $"OperationId should start with the prefix 'Get' or 'List' for operation '{operationValue.OperationId}'."));
                        }
                    }
                    else if (operationKey == OperationType.Post)
                    {
                        if (operationValue.OperationId.StartsWith("Delete", StringComparison.OrdinalIgnoreCase))
                        {
                            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation04, $"OperationId should not start with the prefix 'Delete' for operation '{operationValue.OperationId}'."));
                        }
                    }
                    else if (operationKey == OperationType.Put)
                    {
                        if (!operationValue.OperationId.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
                        {
                            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation05, $"OperationId should start with the prefix 'Update' for operation '{operationValue.OperationId}'."));
                        }
                    }
                    else if (operationKey == OperationType.Patch)
                    {
                        if (!operationValue.OperationId.StartsWith("Patch", StringComparison.OrdinalIgnoreCase) &&
                            !operationValue.OperationId.StartsWith("Update", StringComparison.OrdinalIgnoreCase))
                        {
                            logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation06, $"OperationId should start with the prefix 'Update' for operation '{operationValue.OperationId}'."));
                        }
                    }
                    else if (operationKey == OperationType.Delete &&
                             !operationValue.OperationId.StartsWith("Delete", StringComparison.OrdinalIgnoreCase) &&
                             !operationValue.OperationId.StartsWith("Remove", StringComparison.OrdinalIgnoreCase))
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation07, $"OperationId should start with the prefix 'Delete' for operation '{operationValue.OperationId}'."));
                    }
                }
            }

            if (validationOptions.OperationIdValidation)
            {
                foreach (var (operationKey, operationValue) in pathValue.Operations)
                {
                    // Validate Response Schema
                    var responseModelSchema = operationValue.GetModelSchemaFromResponse();
                    if (responseModelSchema is not null)
                    {
                        if (operationValue.IsOperationIdPluralized(operationKey))
                        {
                            if (!responseModelSchema.IsModelOfTypeArray(modelSchemas))
                            {
                                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation08, $"OperationId '{operationValue.OperationId}' is not singular - Response model is defined as a single item."));
                            }
                        }
                        else
                        {
                            if (responseModelSchema.IsModelOfTypeArray(modelSchemas))
                            {
                                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation09, $"OperationId '{operationValue.OperationId}' is not pluralized - Response model is defined as an array."));
                            }
                        }
                    }

                    //// TO-DO Validate RequestBody Schema
                }
            }
        }
    }

    private void ValidatePathsAndOperations(
        OpenApiPaths paths)
    {
        foreach (var path in paths)
        {
            if (!path.Key.IsStringFormatParametersBalanced(false))
            {
                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Path01, $"Path parameters are not well-formatted for '{path.Key}'."));
            }

            var globalPathParameterNames = path.Value.Parameters
                .Where(x => x.In == ParameterLocation.Path)
                .Select(x => x.Name)
                .ToList();

            if (globalPathParameterNames.Any())
            {
                ValidateGlobalParameters(globalPathParameterNames, path);
            }
            else
            {
                ValidateMissingOperationParametersWhenPresentInPath(path);
                ValidateOperationsWithParametersNotPresentInPath(path);
            }

            ValidateGetOperations(path);
        }
    }

    private void ValidateGlobalParameters(
        IEnumerable<string> globalPathParameterNames,
        KeyValuePair<string, OpenApiPathItem> path)
    {
        foreach (var pathParameterName in globalPathParameterNames)
        {
            if (!path.Key.Contains(pathParameterName, StringComparison.OrdinalIgnoreCase))
            {
                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation11, $"Defined global path parameter '{pathParameterName}' does not exist in route '{path.Key}'."));
            }
        }
    }

    private void ValidateMissingOperationParametersWhenPresentInPath(
        KeyValuePair<string, OpenApiPathItem> path)
    {
        if (!path.Key.Contains('{', StringComparison.Ordinal) ||
            !path.Key.IsStringFormatParametersBalanced(false))
        {
            return;
        }

        var parameterNamesToCheckAgainst = GetParameterListFromPathKey(path.Key);
        var allOperationsParametersFromPath = GetAllOperationsParametersFromPath(path.Value.Operations);
        var distinctOperations = allOperationsParametersFromPath
            .Select(x => x.Item1)
            .Distinct(StringComparer.Ordinal)
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
                    ? logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation12, $"The operations in path '{path.Key}' does not define a parameter named '{parameterName}'.")
                    : logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation12, $"The operations '{string.Join(',', operationsWithMissingParameter)}' in path '{path.Key}' does not define a parameter named '{parameterName}'."));
            }
        }
    }

    private static IEnumerable<string> GetParameterListFromPathKey(
        string pathKey)
    {
        var sa = pathKey
            .Split('{', StringSplitOptions.RemoveEmptyEntries)
            .Where(x => x.Contains('}', StringComparison.Ordinal))
            .ToList();

        return sa
            .Select(item => item.Substring(0, item.IndexOf('}', StringComparison.Ordinal)))
            .ToList();
    }

    private static List<Tuple<string, string>> GetAllOperationsParametersFromPath(
        IDictionary<OperationType, OpenApiOperation> apiOperations)
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

    private void ValidateOperationsWithParametersNotPresentInPath(
        KeyValuePair<string, OpenApiPathItem> path)
    {
        var openApiOperationsWithPathParameter = path.Value.Operations.Values
            .Where(x => x.Parameters.Any(p => p.In == ParameterLocation.Path))
            .ToList();

        if (!openApiOperationsWithPathParameter.Any())
        {
            return;
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
            return;
        }

        foreach (var operationParameterName in operationPathParameterNames)
        {
            if (!path.Key.Contains(operationParameterName, StringComparison.OrdinalIgnoreCase))
            {
                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation13, $"Defined path parameter '{operationParameterName}' does not exist in route '{path.Key}'."));
            }
        }
    }

    private void ValidateGetOperations(
        KeyValuePair<string, OpenApiPathItem> path)
    {
        foreach (var (key, value) in path.Value.Operations)
        {
            if (key != OperationType.Get ||
                (path.Value.Parameters.All(x => x.In != ParameterLocation.Path) &&
                 value.Parameters.All(x => x.In != ParameterLocation.Path)))
            {
                continue;
            }

            var httpStatusCodes = value.Responses.GetHttpStatusCodes();
            if (!httpStatusCodes.Contains(HttpStatusCode.NotFound))
            {
                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation14, $"Missing NotFound response type for operation '{value.GetOperationName()}', required by url parameter."));
            }
        }
    }

    private void ValidateOperationsParametersAndResponses(
        Dictionary<string, OpenApiPathItem>.ValueCollection paths)
    {
        foreach (var path in paths)
        {
            foreach (var (_, value) in path.Operations)
            {
                var httpStatusCodes = value.Responses.GetHttpStatusCodes();
                if (httpStatusCodes.Contains(HttpStatusCode.BadRequest) &&
                    !value.HasParametersOrRequestBody() &&
                    !path.HasParameters())
                {
                    logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation10, $"Contains BadRequest response type for operation '{value.OperationId}', but has no parameters."));
                }

                if (httpStatusCodes.Contains(HttpStatusCode.OK) &&
                    httpStatusCodes.Contains(HttpStatusCode.Created))
                {
                    // We do not support both 200 and 201, since our ActionResult - implicit operators only supports 1 type.
                    logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation18, $"The operation '{value.OperationId}' contains both 200 and 201, which is not supported."));
                }

                if (value.HasParametersOrRequestBody())
                {
                    var schema = value.RequestBody?.Content.GetSchemaByFirstMediaType();
                    if (schema is not null &&
                        string.IsNullOrEmpty(schema.GetModelName()) &&
                        !schema.IsFormatTypeBinary() &&
                        !schema.HasItemsWithFormatTypeBinary())
                    {
                        logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation17, $"RequestBody is defined with inline model for operation '{value.OperationId}' - only reference to component-schemas are supported."));
                    }
                }

                foreach (var parameter in value.Parameters)
                {
                    switch (parameter.In)
                    {
                        case ParameterLocation.Path:
                            if (!parameter.Required)
                            {
                                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation15, $"Path parameter '{parameter.Name}' for operation '{value.OperationId}' is missing required=true."));
                            }

                            if (parameter.Schema.Nullable)
                            {
                                logItems.Add(logItemFactory.Create(logCategory, ValidationRuleNameConstants.Operation16, $"Path parameter '{parameter.Name}' for operation '{value.OperationId}' must not be nullable."));
                            }

                            break;
                        case ParameterLocation.Query:
                            break;
                    }
                }
            }
        }
    }

    private static void AddIndentationToLogItemKeys(
        List<LogKeyValueItem> logItems)
    {
        foreach (var item in logItems)
        {
            item.Key = item.Key.Insert(0, "     ");
        }
    }
}
namespace Atc.Rest.ApiGenerator.CLI.Commands;

public static class ArgumentCommandConstants
{
    public const string ShortOutputPath = "-o";
    public const string LongOutputPath = "--outputPath";
    public const string ShortProjectPrefixName = "-p";
    public const string LongProjectPrefixName = "--projectPrefixName";

    public const string ShortConfigurationSpecificationPath = "-s";
    public const string LongConfigurationSpecificationPath = "--specificationPath";
    public const string LongConfigurationOptionsPath = "--optionsPath";

    public const string LongConfigurationValidateStrictMode = "--validate-strictMode";
    public const string LongConfigurationValidateOperationIdValidation = "--validate-operationIdValidation";
    public const string LongConfigurationValidateOperationIdCasingStyle = "--validate-operationIdCasingStyle";
    public const string LongConfigurationValidateModelNameCasingStyle = "--validate-modelNameCasingStyle";
    public const string LongConfigurationValidateModelPropertyNameCasingStyle = "--validate-modelPropertyNameCasingStyle";

    public const string LongClientFolderName = "--clientFolderName";
    public const string LongClientHttpClientName = "--httpClientName";
    public const string LongExcludeEndpointGeneration = "--excludeEndpointGeneration";

    public const string LongServerOutputSolutionPath = "--outputSlnPath";
    public const string LongServerOutputSourcePath = "--outputSrcPath";
    public const string LongServerOutputTestPath = "--outputTestPath";
    public const string LongServerOutputApiPath = "--apiPath";
    public const string LongServerOutputDomainPath = "--domainPath";
    public const string LongServerDisableCodingRules = "--disableCodingRules";
    public const string LongRemoveNamespaceGroupSeparatorInGlobalUsings = "--removeNamespaceGroupSeparatorInGlobalUsings";
}
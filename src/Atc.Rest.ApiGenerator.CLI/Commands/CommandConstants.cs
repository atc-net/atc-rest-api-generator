namespace Atc.Rest.ApiGenerator.CLI.Commands
{
    public static class CommandConstants
    {
        public const string ArgumentShortHelp = "-h";
        public const string ArgumentLongHelp = "--help";
        public const string ArgumentLongVersion = "--version";
        public const string ArgumentShortVerbose = "-v";
        public const string ArgumentLongVerbose = "--verbose";

        public const string ArgumentShortOutputPath = "-o";
        public const string ArgumentLongOutputPath = "--outputPath";
        public const string ArgumentShortProjectPrefixName = "-p";
        public const string ArgumentLongProjectPrefixName = "--projectPrefixName";

        public const string ArgumentShortConfigurationSpecificationPath = "-s";
        public const string ArgumentLongConfigurationSpecificationPath = "--specificationPath";
        public const string ArgumentLongConfigurationOptionsPath = "--optionsPath";
        public const string ArgumentLongConfigurationAuthorization = "--useAuthorization";

        public const string ArgumentLongConfigurationValidateStrictMode = "--validate-strictMode";
        public const string ArgumentLongConfigurationValidateOperationIdCasingStyle = "--validate-operationIdCasingStyle";
        public const string ArgumentLongConfigurationValidateModelNameCasingStyle = "--validate-modelNameCasingStyle";
        public const string ArgumentLongConfigurationValidateModelPropertyNameCasingStyle = "--validate-modelPropertyNameCasingStyle";

        public const string ArgumentLongClientFolderName = "--clientFolderName";
        public const string ArgumentLongExcludeEndpointGeneration = "--excludeEndpointGeneration";

        public const string ArgumentLongServerOutputSolutionPath = "--outputSlnPath";
        public const string ArgumentLongServerOutputSourcePath = "--outputSrcPath";
        public const string ArgumentLongServerOutputTestPath = "--outputTestPath";
        public const string ArgumentLongServerOutputApiPath = "--apiPath";
        public const string ArgumentLongServerOutputDomainPath = "--domainPath";
        public const string ArgumentLongServerDisableCodingRules = "--disableCodingRules";
    }
}
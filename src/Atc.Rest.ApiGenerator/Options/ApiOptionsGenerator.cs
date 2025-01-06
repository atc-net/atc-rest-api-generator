namespace Atc.Rest.ApiGenerator.Options;

public class ApiOptionsGenerator
{
    public AspNetOutputType AspNetOutputType { get; set; } = AspNetOutputType.Mvc;

    public SwaggerThemeMode SwaggerThemeMode { get; set; } = SwaggerThemeMode.None;

    public bool UseRestExtended { get; set; } = true;

    public bool IncludeDeprecated { get; set; }

    public string ProjectName { get; set; } = string.Empty;

    public string ProjectSuffixName { get; set; } = string.Empty;

    public string ContractsLocation { get; set; } = ContentGeneratorConstants.Contracts;

    public string EndpointsLocation { get; set; } = ContentGeneratorConstants.Endpoints;

    public bool UsePartialClassForContracts { get; set; }

    public bool UsePartialClassForEndpoints { get; set; }

    public bool RemoveNamespaceGroupSeparatorInGlobalUsings { get; set; }

    public ApiOptionsGeneratorRequest Request { get; set; } = new();

    public ApiOptionsGeneratorResponse Response { get; set; } = new();

    public ApiOptionsGeneratorClient? Client { get; set; }

    public override string ToString()
        => $"{nameof(AspNetOutputType)}: {AspNetOutputType}, {nameof(SwaggerThemeMode)}: {SwaggerThemeMode}, {nameof(UseRestExtended)}: {UseRestExtended}, {nameof(IncludeDeprecated)}: {IncludeDeprecated}, {nameof(ProjectName)}: {ProjectName}, {nameof(ProjectSuffixName)}: {ProjectSuffixName}, {nameof(RemoveNamespaceGroupSeparatorInGlobalUsings)}: {RemoveNamespaceGroupSeparatorInGlobalUsings}, {nameof(Request)}: ({Request}), {nameof(Response)}: ({Response}), {nameof(Client)}: ({Client})";
}
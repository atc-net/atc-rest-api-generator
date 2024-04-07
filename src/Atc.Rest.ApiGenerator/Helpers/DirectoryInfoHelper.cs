namespace Atc.Rest.ApiGenerator.Helpers;

public static class DirectoryInfoHelper
{
    public static DirectoryInfo GetProjectPath()
    {
        var currentDomainBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectPath = currentDomainBaseDirectory!
            .Replace("\\Bin", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("\\net8.0", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("\\Debug", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("\\ApiGenerator", string.Empty, StringComparison.OrdinalIgnoreCase);

        return new DirectoryInfo(projectPath!);
    }

    public static string GetCsFileNameForEndpoints(
        DirectoryInfo pathForEndpoints,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(pathForEndpoints);
        ArgumentNullException.ThrowIfNull(modelName);

        return Path.Combine(pathForEndpoints.FullName, $"{modelName}.cs");
    }

    public static string GetCsFileNameForContract(
        DirectoryInfo pathForContracts,
        string apiGroupName,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(pathForContracts);
        ArgumentNullException.ThrowIfNull(apiGroupName);
        ArgumentNullException.ThrowIfNull(modelName);

        var a = Path.Combine(pathForContracts.FullName, apiGroupName);
        var b = Path.Combine(a, $"{modelName}.cs");
        return b;
    }

    public static string GetCsFileNameForContract(
        DirectoryInfo pathForContracts,
        string apiGroupName,
        string subArea,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(pathForContracts);
        ArgumentNullException.ThrowIfNull(apiGroupName);
        ArgumentNullException.ThrowIfNull(subArea);
        ArgumentNullException.ThrowIfNull(modelName);

        var a = Path.Combine(pathForContracts.FullName, apiGroupName);
        var b = Path.Combine(a, subArea);
        var c = Path.Combine(b, $"{modelName}.cs");
        return c;
    }

    public static string GetCsFileNameForContractEnumTypes(
        DirectoryInfo pathForContracts,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(pathForContracts);
        ArgumentNullException.ThrowIfNull(modelName);

        var a = Path.Combine(pathForContracts.FullName, ContentGeneratorConstants.SpecialFolderEnumerationTypes);
        var b = Path.Combine(a, $"{modelName}.cs");
        return b;
    }

    public static string GetCsFileNameForContractShared(
        DirectoryInfo pathForContracts,
        string modelName)
    {
        ArgumentNullException.ThrowIfNull(pathForContracts);
        ArgumentNullException.ThrowIfNull(modelName);

        return Path.Combine(pathForContracts.FullName, $"{modelName}.cs");
    }

    public static string GetCsFileNameForHandler(
        DirectoryInfo pathForHandlers,
        string apiGroupName,
        string handlerName)
    {
        ArgumentNullException.ThrowIfNull(pathForHandlers);
        ArgumentNullException.ThrowIfNull(apiGroupName);
        ArgumentNullException.ThrowIfNull(handlerName);

        var a = Path.Combine(pathForHandlers.FullName, apiGroupName);
        var b = Path.Combine(a, $"{handlerName}.cs");
        return b;
    }
}
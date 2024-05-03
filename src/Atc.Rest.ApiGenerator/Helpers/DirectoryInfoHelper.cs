namespace Atc.Rest.ApiGenerator.Helpers;

public static class DirectoryInfoHelper
{
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
}
namespace Atc.Rest.ApiGenerator.CLI;

public static class CodingRulesHelper
{
    public static bool ShouldScaffoldCodingRules(
        string rootPath,
        bool disableCodingRules)
    {
        ArgumentNullException.ThrowIfNull(rootPath);

        return !disableCodingRules &&
               !HasBuildPropsWithCodingRulesEnabled(rootPath);
    }

    public static bool IsUsingCodingRules(
        string rootPath,
        bool disableCodingRules)
    {
        ArgumentNullException.ThrowIfNull(rootPath);

        return !disableCodingRules &&
               HasBuildPropsWithCodingRulesEnabled(rootPath);
    }

    private static bool HasBuildPropsWithCodingRulesEnabled(
        string rootPath)
    {
        var rootDirectoryBuildProps = new FileInfo(Path.Combine(rootPath, GenerateAtcCodingRulesHelper.FileNameDirectoryBuildProps));
        if (!rootDirectoryBuildProps.Exists)
        {
            return false;
        }

        var fileContent = FileHelper.ReadAllText(rootDirectoryBuildProps);
        return fileContent.Contains("<Nullable>enable</Nullable>", StringComparison.Ordinal);
    }
}
namespace Atc.Rest.ApiGenerator.CodingRules;

public interface IAtcCodingRulesUpdater
{
    bool Scaffold(
        string outputSlnPath,
        DirectoryInfo outputSrcPath,
        DirectoryInfo? outputTestPath);
}
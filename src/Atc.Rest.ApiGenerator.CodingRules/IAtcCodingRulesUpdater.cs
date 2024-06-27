namespace Atc.Rest.ApiGenerator.CodingRules;

public interface IAtcCodingRulesUpdater
{
    bool Scaffold(
        string slnPath,
        DirectoryInfo srcPath,
        DirectoryInfo? testPath);
}
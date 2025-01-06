namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class FileInfoFactory
{
    public static FileInfo Create(
        DirectoryInfo projectPath,
        params string[] subParts)
    {
        if (subParts is null || subParts.Length == 0)
        {
            throw new ArgumentException("At least one path part must be provided.", nameof(subParts));
        }

        var fileName = subParts[^1];

        if (!fileName.Contains('.', StringComparison.Ordinal))
        {
            throw new ArgumentException("The file name must include a valid extension (e.g., .cs).", nameof(subParts));
        }

        var pathParts = subParts[..^1];

        var subPath = string
            .Join('.', pathParts)
            .Trim()
            .Replace('\\', ' ')
            .Replace('-', ' ')
            .Replace('.', ' ')
            .PascalCase(separators: [' ', '-'])
            .Replace("_shared", "_Shared", StringComparison.Ordinal)
            .Replace("_enumerationTypes", "_EnumerationTypes", StringComparison.Ordinal);
        return projectPath.CombineFileInfo(subPath.Split(' ').Concat([fileName]).ToArray());
    }
}
// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Framework.Helpers
{
    public static class GlobalUsingsHelper
    {
        public static void CreateOrUpdate(
            ILogger logger,
            ContentWriterArea contentWriterArea,
            DirectoryInfo directoryInfo,
            IReadOnlyList<string> requiredUsings,
            bool removeNamespaceGroupSeparatorInGlobalUsings)
        {
            ArgumentNullException.ThrowIfNull(directoryInfo);
            ArgumentNullException.ThrowIfNull(requiredUsings);

            if (!requiredUsings.Any())
            {
                return;
            }

            var content = DotnetGlobalUsingsHelper.GetNewContentByReadingExistingIfExistAndMergeWithRequired(
                directoryInfo: directoryInfo,
                requiredNamespaces: requiredUsings,
                setSystemFirst: true,
                addNamespaceSeparator: !removeNamespaceGroupSeparatorInGlobalUsings);

            var globalUsingFile = new FileInfo(Path.Combine(directoryInfo.FullName, "GlobalUsings.cs"));
            if (string.IsNullOrEmpty(content))
            {
                return;
            }

            var contentWriter = new ContentWriter(logger);
            contentWriter.Write(
                globalUsingFile.Directory!,
                globalUsingFile,
                contentWriterArea,
                content);
        }
    }
}
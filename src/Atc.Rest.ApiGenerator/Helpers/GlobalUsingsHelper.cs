// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
// ReSharper disable LoopCanBeConvertedToQuery
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class GlobalUsingsHelper
    {
        public static void CreateOrUpdate(
            ILogger logger,
            string fileDisplayLocation,
            DirectoryInfo directoryInfo,
            List<string> requiredUsings)
        {
            ArgumentNullException.ThrowIfNull(directoryInfo);
            ArgumentNullException.ThrowIfNull(requiredUsings);

            if (!requiredUsings.Any())
            {
                return;
            }

            var content = DotnetGlobalUsingsHelper.GetNewContentByReadingExistingIfExistAndMergeWithRequired(
                directoryInfo,
                requiredUsings);

            var globalUsingFile = new FileInfo(Path.Combine(directoryInfo.FullName, "GlobalUsings.cs"));
            if (!string.IsNullOrEmpty(content))
            {
                TextFileHelper.Save(logger, globalUsingFile, fileDisplayLocation, content);
            }
        }
    }
}
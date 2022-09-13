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

            var rawRequiredUsings = StripToRawUsings(requiredUsings.ToArray());
            var rawExistingUsings = new List<string>();

            var globalUsingFile = new FileInfo(Path.Combine(directoryInfo.FullName, "GlobalUsings.cs"));
            if (globalUsingFile.Exists)
            {
                rawExistingUsings = StripToRawUsings(FileHelper.ReadAllTextToLines(globalUsingFile));
            }

            var rawUsings = MergeRawUsings(rawRequiredUsings, rawExistingUsings);
            var content = GetContentFromRawUsings(rawUsings);

            if (!string.IsNullOrEmpty(content))
            {
                TextFileHelper.Save(logger, globalUsingFile, fileDisplayLocation, content);
            }
        }

        private static string GetContentFromRawUsings(
            IReadOnlyCollection<string> rawUsings,
            bool systemFirst = true,
            bool addNamespaceSeparator = true)
        {
            var sb = new StringBuilder();

            if (systemFirst)
            {
                var rawSortedSystemUsings = rawUsings
                    .Where(x => x.Equals("System", StringComparison.Ordinal) ||
                                x.StartsWith("System.", StringComparison.Ordinal))
                    .OrderBy(x => x)
                    .ToList();

                var rawSortedOtherUsings = rawUsings
                    .Where(x => !x.Equals("System", StringComparison.Ordinal) &&
                                !x.StartsWith("System.", StringComparison.Ordinal))
                    .OrderBy(x => x)
                    .ToList();

                foreach (var item in rawSortedSystemUsings)
                {
                    sb.AppendLine($"global using {item};", GlobalizationConstants.EnglishCultureInfo);
                }

                if (addNamespaceSeparator &&
                    rawSortedSystemUsings.Any() &&
                    rawSortedOtherUsings.Any())
                {
                    sb.AppendLine();
                }

                sb.Append(RawUsingsToContent(addNamespaceSeparator, rawSortedOtherUsings));
            }
            else
            {
                var rawSortedUsings = rawUsings
                    .OrderBy(x => x)
                    .ToList();

                sb.Append(RawUsingsToContent(addNamespaceSeparator, rawSortedUsings));
            }

            return sb.ToString();
        }

        private static string RawUsingsToContent(
            bool addNamespaceSeparator,
            List<string> rawUsings)
        {
            var sb = new StringBuilder();
            if (addNamespaceSeparator)
            {
                sb.Append(RawUsingsToContent(rawUsings));
            }
            else
            {
                foreach (var item in rawUsings)
                {
                    sb.AppendLine($"global using {item};", GlobalizationConstants.EnglishCultureInfo);
                }
            }

            return sb.ToString();
        }

        private static string RawUsingsToContent(
            List<string> rawUsings)
        {
            var sb = new StringBuilder();
            var lastMajorNs = string.Empty;
            foreach (var item in rawUsings)
            {
                if (string.IsNullOrEmpty(lastMajorNs))
                {
                    lastMajorNs = item.Split('.').First();
                }
                else
                {
                    var majorNs = item.Split('.').First();
                    if (!lastMajorNs.Equals(majorNs, StringComparison.Ordinal))
                    {
                        lastMajorNs = majorNs;
                        sb.AppendLine();
                    }
                }

                sb.AppendLine($"global using {item};", GlobalizationConstants.EnglishCultureInfo);
            }

            return sb.ToString();
        }

        private static List<string> StripToRawUsings(
            IEnumerable<string> lines)
        {
            var result = new List<string>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var trimLine = line
                    .Replace("global ", string.Empty, StringComparison.Ordinal)
                    .Replace("using ", string.Empty, StringComparison.Ordinal)
                    .Replace(";", string.Empty, StringComparison.Ordinal)
                    .Trim();

                if (string.IsNullOrWhiteSpace(trimLine))
                {
                    continue;
                }

                result.Add(trimLine);
            }

            return result;
        }

        private static List<string> MergeRawUsings(
            IEnumerable<string> rawRequiredUsings,
            IEnumerable<string> rawExistingUsings)
        {
            var result = new List<string>();

            foreach (var item in rawRequiredUsings.Where(x => !result.Contains(x, StringComparer.Ordinal)))
            {
                result.Add(item);
            }

            foreach (var item in rawExistingUsings.Where(x => !result.Contains(x, StringComparer.Ordinal)))
            {
                result.Add(item);
            }

            return result;
        }
    }
}
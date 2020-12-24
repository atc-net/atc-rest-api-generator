using System;
using System.IO;
using System.Text;
using Atc.Data.Models;

namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class TextFileHelper
    {
        public static LogKeyValueItem Save(string file, string text, bool overrideIfExist = true)
        {
            if (file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            return Save(new FileInfo(file), text, overrideIfExist);
        }

        public static LogKeyValueItem Save(FileInfo fileInfo, string text, bool overrideIfExist = true)
        {
            if (fileInfo == null)
            {
                throw new ArgumentNullException(nameof(fileInfo));
            }

            if (text == null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (fileInfo.Directory != null && !fileInfo.Directory.Exists)
            {
                Directory.CreateDirectory(fileInfo.Directory.FullName);
            }

            // Trim last NewLine in in *.cs files
            if (fileInfo.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase) &&
                text.EndsWith("}" + Environment.NewLine, StringComparison.Ordinal))
            {
                var index = text.LastIndexOf(Environment.NewLine, StringComparison.Ordinal);
                text = text.Remove(index);
            }

            if (File.Exists(fileInfo.FullName))
            {
                if (!overrideIfExist)
                {
                    return new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", fileInfo.FullName);
                }

                if (fileInfo.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase) ||
                    fileInfo.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase) ||
                    fileInfo.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase))
                {
                    // If the content is the same - Note this don't take care of GIT-CRLF handling process.
                    string orgText = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
                    if (orgText == text)
                    {
                        return new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", fileInfo.FullName);
                    }

                    if (RemoveApiGeneratorVersionLine(orgText, true) == RemoveApiGeneratorVersionLine(text, true))
                    {
                        return new LogKeyValueItem(LogCategoryType.Debug, "FileSkip", "#", fileInfo.FullName);
                    }
                }

                File.WriteAllText(fileInfo.FullName, text, Encoding.UTF8);
                return new LogKeyValueItem(LogCategoryType.Debug, "FileUpdate", "#", fileInfo.FullName);
            }

            File.WriteAllText(fileInfo.FullName, text, Encoding.UTF8);
            return new LogKeyValueItem(LogCategoryType.Debug, "FileCreate", "#", fileInfo.FullName);
        }

        private static string RemoveApiGeneratorVersionLine(string text, bool removeNewLines = false)
        {
            string[] sa = text.Split(Environment.NewLine.ToCharArray());
            var sb = new StringBuilder();
            bool isRemovedComment = false;
            bool isRemovedAttribute = false;
            int lastIndex = sa.Length - 1;
            for (int i = 0; i < sa.Length; i++)
            {
                if (!isRemovedComment &&
                    sa[i].Contains("auto-generated", StringComparison.Ordinal) &&
                    sa[i].Contains("ApiGenerator", StringComparison.Ordinal))
                {
                    isRemovedComment = true;
                    continue;
                }

                if (!isRemovedAttribute &&
                    sa[i].Contains("GeneratedCode", StringComparison.Ordinal) &&
                    sa[i].Contains("ApiGenerator", StringComparison.Ordinal))
                {
                    isRemovedAttribute = true;
                    continue;
                }

                if (removeNewLines)
                {
                    sb.Append(sa[i]);
                }
                else
                {
                    if (i == lastIndex)
                    {
                        sb.Append(sa[i]);
                    }
                    else
                    {
                        sb.AppendLine(sa[i]);
                    }
                }
            }

            return sb.ToString();
        }
    }
}
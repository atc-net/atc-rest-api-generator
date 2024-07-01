// ReSharper disable SuggestBaseTypeForParameter

namespace Atc.Rest.ApiGenerator.Framework.Writers;

public sealed class ContentWriter : IContentWriter
{
    private readonly ILogger logger;

    public ContentWriter(
        ILogger logger)
    {
        this.logger = logger;
    }

    public void Write(
        DirectoryInfo workingFolder,
        FileInfo file,
        ContentWriterArea contentWriterArea,
        string content,
        bool overrideIfExist = true)
    {
        ArgumentNullException.ThrowIfNull(workingFolder);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(content);

        if (file.Directory is not null &&
            !file.Directory.Exists)
        {
            Directory.CreateDirectory(file.Directory.FullName);
        }

        var logAreaFileMessage = GetLogAreaFileMessage(workingFolder, file, contentWriterArea);

        content = RemoveLastNewLineIfNeeded(file, content);

        if (File.Exists(file.FullName))
        {
            if (!overrideIfExist)
            {
                logger.LogTrace($"{ContentWriterConstants.LogFileNotUpdated}   {logAreaFileMessage} nothing to update");
                return;
            }

            if (file.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase) ||
                file.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase) ||
                file.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase))
            {
                // If the content is the same - Note this don't take care of GIT-CRLF handling process.
                var orgText = FileHelper.ReadAllText(file);
                if (AreContentsEqual(content, orgText))
                {
                    logger.LogTrace($"{ContentWriterConstants.LogFileNotUpdated}   {logAreaFileMessage} nothing to update");
                    return;
                }
            }

            FileHelper.WriteAllText(file, content);
            logger.LogDebug($"{ContentWriterConstants.LogFileUpdated}   {logAreaFileMessage} updated");
            return;
        }

        FileHelper.WriteAllText(file, content);
        logger.LogDebug($"{ContentWriterConstants.LogFileCreated}   {logAreaFileMessage} created");
    }

    private static string GetLogAreaFileMessage(
        DirectoryInfo workingFolder,
        FileInfo file,
        ContentWriterArea contentWriterArea)
    {
        var logAreaFileMessage = file.FullName.Replace(
            workingFolder.FullName,
            GetContentWriterAreaForLog(contentWriterArea),
            StringComparison.Ordinal);

        return contentWriterArea switch
        {
            ContentWriterArea.Root => logAreaFileMessage.Replace("root: \\", "root: ", StringComparison.Ordinal),
            ContentWriterArea.Src => logAreaFileMessage.Replace("src:  \\", "src:  ", StringComparison.Ordinal),
            ContentWriterArea.Test => logAreaFileMessage.Replace("test: \\", "test: ", StringComparison.Ordinal),
            _ => throw new SwitchCaseDefaultException(contentWriterArea),
        };
    }

    private static string GetContentWriterAreaForLog(
        ContentWriterArea contentWriterArea)
        => contentWriterArea switch
        {
            ContentWriterArea.Root => "root: ",
            ContentWriterArea.Src => "src:  ",
            ContentWriterArea.Test => "test: ",
            _ => throw new SwitchCaseDefaultException(contentWriterArea),
        };

    private static string RemoveLastNewLineIfNeeded(
        FileInfo fileInfo,
        string text)
    {
        if ((fileInfo.Extension.Equals(".cs", StringComparison.OrdinalIgnoreCase) && text.EndsWith("}" + Environment.NewLine, StringComparison.Ordinal)) ||
            (fileInfo.Name.Equals("GlobalUsings.cs", StringComparison.Ordinal) && text.EndsWith(";" + Environment.NewLine, StringComparison.Ordinal)))
        {
            // Trim last NewLine in *.cs files or last NewLine in GlobalUsings.cs files
            text = RemoveLastNewLine(text);
        }

        return text;
    }

    private static string RemoveLastNewLine(
        string text)
        => text.Remove(text.LastIndexOf(Environment.NewLine, StringComparison.Ordinal));

    private static bool AreContentsEqual(
        string content,
        string orgText)
        => orgText == content ||
           RemoveApiGeneratorVersionLine(orgText, removeNewLines: true) == RemoveApiGeneratorVersionLine(content, removeNewLines: true);

    private static string RemoveApiGeneratorVersionLine(
        string text,
        bool removeNewLines = false)
    {
        var sa = text.Split(Environment.NewLine.ToCharArray());
        var sb = new StringBuilder();
        var isCommentRemoved = false;
        var isAttributeRemoved = false;
        var lastIndex = sa.Length - 1;

        for (var i = 0; i < sa.Length; i++)
        {
            if (!isCommentRemoved &&
                sa[i].Contains("auto-generated", StringComparison.Ordinal) &&
                sa[i].Contains(ContentWriterConstants.ApiGeneratorName, StringComparison.Ordinal))
            {
                isCommentRemoved = true;
                continue;
            }

            if (!isAttributeRemoved &&
                sa[i].Contains("GeneratedCode", StringComparison.Ordinal) &&
                sa[i].Contains(ContentWriterConstants.ApiGeneratorName, StringComparison.Ordinal))
            {
                isAttributeRemoved = true;
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
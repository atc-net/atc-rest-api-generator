namespace Atc.Rest.ApiGenerator.Framework.Factories;

public static class CodeDocumentationTagsFactory
{
    public static CodeDocumentationTags Create(
        string summery)
    {
        return new CodeDocumentationTags(
            summery,
            Parameters: null,
            Example: null,
            Exception: null,
            Return: null);
    }
}
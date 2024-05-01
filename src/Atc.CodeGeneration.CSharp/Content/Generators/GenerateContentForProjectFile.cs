namespace Atc.CodeGeneration.CSharp.Content.Generators;

public class GenerateContentForProjectFile : IContentGenerator
{
    private readonly ProjectFileParameters parameters;

    public GenerateContentForProjectFile(
        ProjectFileParameters parameters)
    {
        this.parameters = parameters;
    }

    public string Generate()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"<Project Sdk=\"{parameters.ProjectSdK}\">");
        sb.AppendLine();

        for (var i = 0; i < parameters.PropertyGroups.Count; i++)
        {
            if (i > 0)
            {
                sb.AppendLine();
            }

            var propertyGroups = parameters.PropertyGroups[i];

            sb.AppendLine(2, "<PropertyGroup>");
            foreach (var propertyGroup in propertyGroups)
            {
                sb.AppendLine(4, $"<{propertyGroup.Key}>{propertyGroup.Value}</{propertyGroup.Key}>");
            }

            sb.AppendLine(2, "</PropertyGroup>");
        }

        if (parameters.PropertyGroups.Count > 0)
        {
            sb.AppendLine();
        }

        for (var i = 0; i < parameters.ItemGroups.Count; i++)
        {
            if (i > 0)
            {
                sb.AppendLine();
            }

            var itemGroups = parameters.ItemGroups[i];

            sb.AppendLine(2, "<ItemGroup>");
            foreach (var itemGroup in itemGroups)
            {
                string? attributes = null;
                if (itemGroup.Attributes is not null)
                {
                    attributes = itemGroup
                        .Attributes
                        .Select(attr => $"{attr.Key}=\"{attr.Value}\"")
                        .Aggregate((current, next) => $"{current} {next}");
                }

                if (string.IsNullOrEmpty(itemGroup.Value))
                {
                    sb.AppendLine(
                        4,
                        attributes is null
                            ? $"<{itemGroup.Key} />"
                            : $"<{itemGroup.Key} {attributes} />");
                }
                else
                {
                    if (itemGroup.Value.StartsWith('<') &&
                        itemGroup.Value.EndsWith('>'))
                    {
                        var xElement = XElement
                            .Parse($"<{itemGroup.Key} {attributes}>{itemGroup.Value}</{itemGroup.Key}>");

                        var lines = xElement
                            .ToString(SaveOptions.None)
                            .ToLines();

                        foreach (var line in lines)
                        {
                            sb.AppendLine(4, line);
                        }
                    }
                    else
                    {
                        sb.AppendLine(4, $"<{itemGroup.Key} {attributes}>{itemGroup.Value}</{itemGroup.Key}>");
                    }
                }
            }

            sb.AppendLine(2, "</ItemGroup>");
        }

        sb.AppendLine();
        sb.AppendLine("</Project>");

        return sb.ToString();
    }
}
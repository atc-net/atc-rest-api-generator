namespace Atc.Rest.ApiGenerator.Factories;

public static class ProjectDomainFactory
{
    public static string[] CreateUsingListForHandler(
        DomainProjectOptions domainProjectOptions,
        string focusOnSegmentName)
    {
        var list = new List<string>
        {
            "System.Threading",
            "System.Threading.Tasks",
            $"{domainProjectOptions.ProjectName.Replace(".Domain", ".Api", StringComparison.Ordinal)}.Generated.{NameConstants.Contracts}.{focusOnSegmentName.EnsureFirstCharacterToUpper()}",
        };

        return list.ToArray();
    }
}
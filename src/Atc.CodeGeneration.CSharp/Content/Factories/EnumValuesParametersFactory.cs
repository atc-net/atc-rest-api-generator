namespace Atc.CodeGeneration.CSharp.Content.Factories;

public static class EnumValuesParametersFactory
{
    public static IList<EnumValueParameters> Create(
        string[] names)
        => names
            .Select(x => EnumValueParametersFactory.Create(x))
            .ToList();

    public static IList<EnumValueParameters> Create(
        IDictionary<string, int?> nameValues)
        => nameValues
            .Select(x => EnumValueParametersFactory.Create(x.Key, x.Value))
            .ToList();
}
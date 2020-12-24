namespace Atc.Rest.ApiGenerator.Factories
{
    internal static class ProjectContractInterfaceFactory
    {
        public static string[] CreateUsingList()
        {
            return new[]
            {
                "System.CodeDom.Compiler",
                "System.Threading",
                "System.Threading.Tasks",
            };
        }
    }
}
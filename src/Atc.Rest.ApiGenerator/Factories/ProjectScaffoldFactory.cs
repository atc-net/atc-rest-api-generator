namespace Atc.Rest.ApiGenerator.Factories
{
    internal static class ProjectScaffoldFactory
    {
        public static string[] CreateUsingListForResultFactory()
        {
            return new[]
            {
                "System.CodeDom.Compiler",
                "System.Net",
                "System.Text.Json",
                "Microsoft.AspNetCore.Mvc",
            };
        }

        public static string[] CreateUsingListForPagination()
        {
            return new[]
            {
                "System",
                "System.CodeDom.Compiler",
                "System.Collections.Generic",
            };
        }
    }
}
using System;
using System.Collections.Generic;

// ReSharper disable InvertIf
namespace Atc.Rest.ApiGenerator.Helpers
{
    public static class NugetPackageReferenceHelper
    {
        public static List<Tuple<string, string, string?>> CreateForHostProject(bool useRestExtended)
        {
            string atcVersion = GenerateHelper.GetAtcToolVersionAsString3();

            var packageReference = new List<Tuple<string, string, string?>>
            {
                new Tuple<string, string, string?>("Atc", atcVersion, null),
                new Tuple<string, string, string?>("Atc.Rest", atcVersion, null),
            };

            if (useRestExtended)
            {
                packageReference.Add(new Tuple<string, string, string?>("Atc.Rest.Extended", atcVersion, null));
                packageReference.Add(new Tuple<string, string, string?>("FluentValidation.AspNetCore", "9.2.0", null));
                packageReference.Add(new Tuple<string, string, string?>("Microsoft.ApplicationInsights.AspNetCore", "2.15.0", null));
                packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Authentication.JwtBearer", "3.1.8", null));
                packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Mvc.Versioning", "4.1.1", null));
                packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer", "4.1.1", null));
                packageReference.Add(new Tuple<string, string, string?>("Swashbuckle.AspNetCore", "5.6.3", null));
            }

            return packageReference;
        }

        public static List<Tuple<string, string, string?>> CreateForApiProject()
        {
            string atcVersion = GenerateHelper.GetAtcToolVersionAsString3();

            var packageReference = new List<Tuple<string, string, string?>>
            {
                new Tuple<string, string, string?>("Atc", atcVersion, null),
                new Tuple<string, string, string?>("Atc.Rest", atcVersion, null),
            };

            return packageReference;
        }

        public static List<Tuple<string, string, string?>> CreateForTestProject(bool useMvc)
        {
            var packageReference = new List<Tuple<string, string, string?>>
            {
                new Tuple<string, string, string?>("AutoFixture", "4.14.0", null),
                new Tuple<string, string, string?>("AutoFixture.AutoNSubstitute", "4.14.0", null),
                new Tuple<string, string, string?>("AutoFixture.Xunit2", "4.14.0", null),
                new Tuple<string, string, string?>("FluentAssertions", "5.10.3", null),
            };

            if (useMvc)
            {
                packageReference.Add(new Tuple<string, string, string?>("Microsoft.AspNetCore.Mvc.Testing", "3.1.8", null));
            }

            packageReference.AddRange(new List<Tuple<string, string, string?>>
            {
                new Tuple<string, string, string?>("Microsoft.NET.Test.Sdk", "16.7.1", null),
                new Tuple<string, string, string?>("NSubstitute", "4.2.2", null),
                new Tuple<string, string, string?>("xunit", "2.4.1", null),
                new Tuple<string, string, string?>("xunit.runner.visualstudio", "2.4.3", "<PrivateAssets>all</PrivateAssets>\n<IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>"),
            });

            return packageReference;
        }
    }
}
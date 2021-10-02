﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Tests.Endpoints.List.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    [Collection("Sequential-Endpoints")]
    public class GetListOfStringsTests : WebApiControllerBaseTest
    {
        public GetListOfStringsTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/list/string")]
        public async Task GetListOfStrings_Ok(string relativeRef)
        {
            // Act
            var response = await HttpClient.GetAsync(relativeRef);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
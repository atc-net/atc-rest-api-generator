﻿using System.CodeDom.Compiler;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Atc.XUnit;
using FluentAssertions;
using Scenario2.Api.Generated.Contracts.Eventargs;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Tests.Endpoints.Eventargs.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    [Collection("Sequential-Endpoints")]
    [Trait(Traits.Category, Traits.Categories.Integration)]
    public class GetEventArgsByIdTests : WebApiControllerBaseTest
    {
        public GetEventArgsByIdTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/eventArgs/27")]
        public async Task GetEventArgsById_Ok(string relativeRef)
        {
            // Act
            var response = await HttpClient.GetAsync(relativeRef);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.DeserializeAsync<Scenario2.Api.Generated.Contracts.Eventargs.EventArgs>(JsonSerializerOptions);
            responseData.Should().NotBeNull();
        }
    }
}
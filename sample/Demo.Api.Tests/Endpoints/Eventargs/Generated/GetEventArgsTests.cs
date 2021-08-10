﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts.Eventargs;
using FluentAssertions;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.124.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Eventargs.Generated
{
    [GeneratedCode("ApiGenerator", "1.1.124.0")]
    [Collection("Sequential-Endpoints")]
    public class GetEventArgsTests : WebApiControllerBaseTest
    {
        public GetEventArgsTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/eventArgs")]
        public async Task GetEventArgs_Ok(string relativeRef)
        {
            // Act
            var response = await HttpClient.GetAsync(relativeRef);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.DeserializeAsync<List<Demo.Api.Generated.Contracts.Eventargs.EventArgs>>(JsonSerializerOptions);
            responseData.Should().NotBeNull();
        }
    }
}
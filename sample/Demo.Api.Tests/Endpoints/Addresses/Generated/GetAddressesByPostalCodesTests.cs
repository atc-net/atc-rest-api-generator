﻿using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts;
using Demo.Api.Generated.Contracts.Addresses;
using FluentAssertions;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.154.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Addresses.Generated
{
    [GeneratedCode("ApiGenerator", "1.1.154.0")]
    [Collection("Sequential-Endpoints")]
    public class GetAddressesByPostalCodesTests : WebApiControllerBaseTest
    {
        public GetAddressesByPostalCodesTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/addresses/Hallo")]
        public async Task GetAddressesByPostalCodes_Ok(string relativeRef)
        {
            // Act
            var response = await HttpClient.GetAsync(relativeRef);

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.DeserializeAsync<List<Address>>(JsonSerializerOptions);
            responseData.Should().NotBeNull();
        }
    }
}
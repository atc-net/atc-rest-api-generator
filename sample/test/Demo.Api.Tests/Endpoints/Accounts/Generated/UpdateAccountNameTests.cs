﻿using System.CodeDom.Compiler;
using System.Net;
using System.Threading.Tasks;
using Atc.XUnit;
using FluentAssertions;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.130.4357.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Accounts.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.130.4357")]
    [Collection("Sequential-Endpoints")]
    [Trait(Traits.Category, Traits.Categories.Integration)]
    public class UpdateAccountNameTests : WebApiControllerBaseTest
    {
        public UpdateAccountNameTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/accounts/77a33260-0000-441f-ba60-b0a833803fab/name")]
        public async Task UpdateAccountName_Ok(string relativeRef)
        {
            // Arrange
            HttpClient.DefaultRequestHeaders.Add("name", "Hallo");

            var data = "{ }";

            // Act
            var response = await HttpClient.PutAsync(relativeRef, ToJson(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/api/v1/accounts/x77a33260-0000-441f-ba60-b0a833803fab/name")]
        public async Task UpdateAccountName_BadRequest_InPath(string relativeRef)
        {
            // Arrange
            HttpClient.DefaultRequestHeaders.Add("name", "Hallo");

            var data = "{ }";

            // Act
            var response = await HttpClient.PutAsync(relativeRef, ToJson(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
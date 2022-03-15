﻿using System.CodeDom.Compiler;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Atc.XUnit;
using FluentAssertions;
using Scenario2.Api.Generated.Contracts;
using Scenario2.Api.Generated.Contracts.Users;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Api.Tests.Endpoints.Users.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    [Collection("Sequential-Endpoints")]
    [Trait(Traits.Category, Traits.Categories.Integration)]
    public class UpdateMyTestGenderTests : WebApiControllerBaseTest
    {
        public UpdateMyTestGenderTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/users/77a33260-0000-441f-ba60-b0a833803fab/gender")]
        public async Task UpdateMyTestGender_Ok(string relativeRef)
        {
            // Arrange
            var data = new UpdateTestGenderRequest
            {
                Gender = GenderType.Female,
            };

            // Act
            var response = await HttpClient.PutAsync(relativeRef, ToJson(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/api/v1/users/x77a33260-0000-441f-ba60-b0a833803fab/gender")]
        public async Task UpdateMyTestGender_BadRequest_InPath(string relativeRef)
        {
            // Arrange
            var data = new UpdateTestGenderRequest
            {
                Gender = GenderType.Female,
            };

            // Act
            var response = await HttpClient.PutAsync(relativeRef, ToJson(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
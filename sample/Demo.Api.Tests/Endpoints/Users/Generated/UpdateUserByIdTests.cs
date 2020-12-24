﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Demo.Api.Generated.Contracts;
using Demo.Api.Generated.Contracts.Users;
using FluentAssertions;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.0.216.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Users.Generated
{
    [GeneratedCode("ApiGenerator", "1.0.216.0")]
    [Collection("Sequential-Endpoints")]
    public class UpdateUserByIdTests : WebApiControllerBaseTest
    {
        public UpdateUserByIdTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/users/77a33260-0000-441f-ba60-b0a833803fab")]
        public async Task UpdateUserById_Ok(string relativeRef)
        {
            // Arrange
            var data = new UpdateUserRequest
            {
                FirstName = "Hallo",
                LastName = "Hallo1",
                Email = "john.doe@example.com",
                Gender = GenderType.Female,
            };

            // Act
            var response = await HttpClient.PutAsync(relativeRef, ToJson(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("/api/v1/users/x77a33260-0000-441f-ba60-b0a833803fab")]
        public async Task UpdateUserById_BadRequest_InPath(string relativeRef)
        {
            // Arrange
            var data = new UpdateUserRequest
            {
                FirstName = "Hallo",
                LastName = "Hallo1",
                Email = "john.doe@example.com",
                Gender = GenderType.Female,
            };

            // Act
            var response = await HttpClient.PutAsync(relativeRef, ToJson(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("/api/v1/users/77a33260-0000-441f-ba60-b0a833803fab")]
        public async Task UpdateUserById_BadRequest_InBody_Email(string relativeRef)
        {
            // Arrange
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"FirstName\": \"Hallo\",");
            sb.AppendLine("  \"LastName\": \"Hallo\"1,");
            sb.AppendLine("  \"Email\": \"john.doe_example.com\",");
            sb.AppendLine("  \"Gender\": \"Female\"");
            sb.AppendLine("}");
            var data = sb.ToString();

            // Act
            var response = await HttpClient.PutAsync(relativeRef, Json(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
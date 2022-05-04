﻿using System.CodeDom.Compiler;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Atc.XUnit;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.130.4357.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests.Endpoints.Files.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.130.4357")]
    [Collection("Sequential-Endpoints")]
    [Trait(Traits.Category, Traits.Categories.Integration)]
    public class UploadSingleFileAsFormDataTests : WebApiControllerBaseTest
    {
        public UploadSingleFileAsFormDataTests(WebApiStartupFactory fixture) : base(fixture) { }

        [Theory]
        [InlineData("/api/v1/files/form-data/singleFile")]
        public async Task UploadSingleFileAsFormData_Ok(string relativeRef)
        {
            // Arrange
            var data = GetTestFile();

            // Act
            var response = await HttpClient.PostAsync(relativeRef, await GetMultipartFormDataContentFromUploadSingleFileAsFormDataRequest(data));

            // Assert
            response.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private async Task<MultipartFormDataContent> GetMultipartFormDataContentFromUploadSingleFileAsFormDataRequest(IFormFile request)
        {
            var formDataContent = new MultipartFormDataContent();
            if (request is not null)
            {
                var bytesContent = new ByteArrayContent(await request.GetBytes());
                formDataContent.Add(bytesContent, "Request", request.FileName);
            }

            return formDataContent;
        }
    }
}
﻿using System.CodeDom.Compiler;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.0.181.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Api.Tests
{
    [GeneratedCode("ApiGenerator", "1.0.181.0")]
    public abstract class WebApiControllerBaseTest : IClassFixture<WebApiStartupFactory>
    {
        protected readonly WebApiStartupFactory Factory;
        protected readonly HttpClient HttpClient;
        protected readonly IConfiguration Configuration;
        protected static JsonSerializerOptions JsonSerializerOptions;
        protected WebApiControllerBaseTest(WebApiStartupFactory fixture)
        {
            this.Factory = fixture;
            this.HttpClient = this.Factory.CreateClient();
            this.Configuration = new ConfigurationBuilder().Build();
            JsonSerializerOptions = new JsonSerializerOptions{PropertyNameCaseInsensitive = true, Converters = {new JsonStringEnumConverter()}, };
        }

        protected static StringContent ToJson(object data) => new StringContent(JsonSerializer.Serialize(data, JsonSerializerOptions), Encoding.UTF8, "application/json");
        protected static StringContent Json(string data) => new StringContent(data, Encoding.UTF8, "application/json");
    }
}
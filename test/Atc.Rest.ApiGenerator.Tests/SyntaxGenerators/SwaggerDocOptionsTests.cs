using System;
using Atc.Rest.ApiGenerator.SyntaxGenerators.Api;
using Microsoft.OpenApi.Models;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.SyntaxGenerators
{
    public class SwaggerDocOptionsTests
    {
        [Fact]
        public void ToCodeAsString_Should_Return_NotNullOrEmpty()
        {
            var sut = new SyntaxGeneratorSwaggerDocDocOptions(
                GetType().Namespace,
                new OpenApiDocument
                {
                    Info = new OpenApiInfo()
                    {
                        Contact = new OpenApiContact()
                        {
                            Email = "apiteam@swagger.io",
                        },
                        Description = @"
This is a sample Pet Store Server based on the OpenAPI 3.0 specification.  You can find out more about
    Swagger at [http://swagger.io](http://swagger.io). In the third iteration of the pet store, we've switched to the design first approach!
    You can now help us improve the API whether it's by making changes to the definition itself or to the code.
    That way, with time, we can improve the API in general, and expose some of the new features in OAS3.

    Some useful links:
    - [The Pet Store repository](https://github.com/swagger-api/swagger-petstore)
    - [The source API definition for the Pet Store](https://github.com/swagger-api/swagger-petstore/blob/master/src/main/resources/openapi.yaml)",
                        License = new OpenApiLicense
                        {
                            Name = "Apache 2.0",
                            Url = new Uri("http://www.apache.org/licenses/LICENSE-2.0.html"),
                        },
                        Title = "Swagger Petstore - OpenAPI 3.0",
                        Version = "1.0.6",
                        TermsOfService = new Uri("http://swagger.io/terms/"),
                    },
                }
            );

            Assert.False(string.IsNullOrWhiteSpace(sut.GenerateCode()));
        }
    }
}
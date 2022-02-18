using System;
using System.CodeDom.Compiler;
using Microsoft.Extensions.Logging;
using Scenario2.Domain.Handlers.Users;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Domain.Tests.Handlers.Users.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class UpdateUserByIdHandlerGeneratedTests
    {
        [Fact]
        public void InstantiateConstructor()
        {
            // Act
            var actual = new UpdateUserByIdHandler();

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void ParameterArgumentNullCheck()
        {
            // Arrange
            var sut = new UpdateUserByIdHandler();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));
        }
    }
}
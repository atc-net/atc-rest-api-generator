﻿//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.259.29354.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Domain.Tests.Handlers.Addresses.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.259.29354")]
    public class GetAddressesByPostalCodesHandlerGeneratedTests
    {
        [Fact]
        public void InstantiateConstructor()
        {
            // Act
            var actual = new GetAddressesByPostalCodesHandler();

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void ParameterArgumentNullCheck()
        {
            // Arrange
            var sut = new GetAddressesByPostalCodesHandler();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));
        }
    }
}
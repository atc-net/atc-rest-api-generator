﻿using System;
using System.CodeDom.Compiler;
using Scenario1.Domain.Handlers.Addresses;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario1.Domain.Tests.Handlers.Addresses.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
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
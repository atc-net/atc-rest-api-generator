﻿using System;
using System.CodeDom.Compiler;
using Demo.Domain.Handlers.Users;
using Microsoft.Extensions.Logging;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 2.0.102.20806.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Domain.Tests.Handlers.Users.Generated
{
    [GeneratedCode("ApiGenerator", "2.0.102.20806")]
    public class PostUserHandlerGeneratedTests
    {
        [Fact]
        public void InstantiateConstructor()
        {
            // Act
            var actual = new PostUserHandler();

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void ParameterArgumentNullCheck()
        {
            // Arrange
            var sut = new PostUserHandler();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));
        }
    }
}
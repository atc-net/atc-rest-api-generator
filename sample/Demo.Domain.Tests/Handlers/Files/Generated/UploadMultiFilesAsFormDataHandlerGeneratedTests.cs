﻿using System;
using System.CodeDom.Compiler;
using Demo.Domain.Handlers.Files;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator 1.1.154.0.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Demo.Domain.Tests.Handlers.Files.Generated
{
    [GeneratedCode("ApiGenerator", "1.1.154.0")]
    public class UploadMultiFilesAsFormDataHandlerGeneratedTests
    {
        [Fact]
        public void InstantiateConstructor()
        {
            // Act
            var actual = new UploadMultiFilesAsFormDataHandler();

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void ParameterArgumentNullCheck()
        {
            // Arrange
            var sut = new UploadMultiFilesAsFormDataHandler();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));
        }
    }
}
﻿using System;
using System.CodeDom.Compiler;
using Scenario2.Domain.Handlers.Files;
using Xunit;

//------------------------------------------------------------------------------
// This code was auto-generated by ApiGenerator x.x.x.x.
//
// Changes to this file may cause incorrect behavior and will be lost if
// the code is regenerated.
//------------------------------------------------------------------------------
namespace Scenario2.Domain.Tests.Handlers.Files.Generated
{
    [GeneratedCode("ApiGenerator", "x.x.x.x")]
    public class UploadSingleObjectWithFilesAsFormDataHandlerGeneratedTests
    {
        [Fact]
        public void InstantiateConstructor()
        {
            // Act
            var actual = new UploadSingleObjectWithFilesAsFormDataHandler();

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void ParameterArgumentNullCheck()
        {
            // Arrange
            var sut = new UploadSingleObjectWithFilesAsFormDataHandler();

            // Act & Assert
            Assert.ThrowsAsync<ArgumentException>(() => sut.ExecuteAsync(null!));
        }
    }
}
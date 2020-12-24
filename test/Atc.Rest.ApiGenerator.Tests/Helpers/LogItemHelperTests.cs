using Atc.Rest.ApiGenerator.Helpers;
using Xunit;

namespace Atc.Rest.ApiGenerator.Tests.Helpers
{
    public class LogItemHelperTests
    {
        [Fact]
        public void Create()
        {
            // Act
            var actual = LogItemHelper.Create(LogCategoryType.Error, ValidationRuleNameConstants.Operation04, "Hallo world");

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Key: CR0204, Value: Operation, LogCategory: Error, Description: Hallo world", actual.ToString());
        }
    }
}
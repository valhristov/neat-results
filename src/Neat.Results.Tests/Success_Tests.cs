using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neat.Results.Tests
{
    [TestClass]
    public class Success_Tests
    {
        [TestMethod]
        public void Convert_Success_To_Value()
        {
            // Arrange
            var result = Result.Success(5);

            // Act & Assert
            result.ValueOrDefault(10).Should().Be(5);

            result.ValueOrThrow().Should().Be(5);

            result.ValueOrThrow(() => new InvalidOperationException()).Should().Be(5);

            result.Value(value => "5", errors => "").Should().Be("5");
        }

        [TestMethod]
        public void Convert_Success_To_Another_Success()
        {
            // Arrange
            var original = Result.Success(5);

            // Act
            var result = original.Select(value => Result.Success("a"));

            // Assert
            result.ValueOrThrow().Should().Be("a");
        }

        [TestMethod]
        public void Convert_Success_To_Failure()
        {
            // Arrange
            var original = Result.Success(5);

            // Act & Assert
            var result = original.Select(value => Result.Failure<string>("error"));

            // Assert
            result.ErrorsOrEmpty().Should().BeEquivalentTo("error");
        }

        [TestMethod]
        public void Match_Value()
        {
            var result = Result.Success(5);

            result.Match(
                value => value.Should().Be(5),
                errors => Assert.Fail("This should not be executed"));
        }
    }
}

using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Neat.Results.Tests
{
    [TestClass]
    public class Failure_Tests
    {
        [TestMethod]
        public void Convert_Failure_To_Value()
        {
            // Arrange
            var result = Result.Failure<int>("error");

            // Act & Assert
            result.ValueOrDefault(10).Should().Be(10);

            Action act;
            act = () => result.ValueOrThrow();
            act.Should().Throw<InvalidOperationException>();

            act = () => result.ValueOrThrow(() => new NotSupportedException());
            act.Should().Throw<NotSupportedException>();

            result.Value(value => "success", errors => "5").Should().Be("5");
        }

        [TestMethod]
        public void Convert_Failure_To_Errors()
        {
            // Arrange
            var result = Result.Failure<int>("error1", "error2");

            // Act & Assert
            result.ErrorsOrEmpty().Should().BeEquivalentTo("error1", "error2");
        }

        [TestMethod]
        public void Convert_Failure_To_Success()
        {
            // Arrange
            var result = Result.Failure<int>("error");

            // Act
            result = result.Select(
                value => throw new Exception(),
                errors => Result.Success(5)); // We are going through here

            // Assert
            result.ValueOrThrow().Should().Be(5);
        }

        [TestMethod]
        public void Convert_Failure_To_Another_Failure()
        {
            // Arrange
            var result = Result.Failure<int>("error");

            // Act & Assert
            result = result.Select(
                value => throw new Exception(),
                errors => Result.Failure<int>(errors.Add("new error"))); // We are going through here

            // Assert
            result.ErrorsOrEmpty().Should().BeEquivalentTo("error", "new error");
        }

        [TestMethod]
        public void Match_Value()
        {
            var result = Result.Failure<int>("error");

            result.Match(
                value => Assert.Fail("This should not be executed"),
                errors => errors.Should().BeEquivalentTo("error"));
        }
    }
}

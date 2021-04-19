using System;
using System.Threading.Tasks;
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
            // Arrange
            var result = Result.Success(5);

            // Act & assert
            result.Match(
                value => value.Should().Be(5),
                errors => Assert.Fail("This should not be executed"));
        }

        [TestMethod]
        public async Task Convert_Success_To_Another_Success_Async()
        {
            // Arrange
            var result = Result.Success(5);

            // Act
            var val = await result.SelectAsync(
                DelayedToString,
                errors => throw new Exception("This should not be executed"));

            // Assert
            val.ValueOrThrow().Should().Be("5");
            val.ErrorsOrEmpty().Should().BeEmpty();

            async Task<Result<string>> DelayedToString(int value)
            {
                await Task.Delay(1);
                return Result.Success(value.ToString());
            }
        }

        [TestMethod]
        public async Task Convert_Success_To_Failure_Async()
        {
            // Arrange
            var result = Result.Success(5);

            // Act
            var val = await result.SelectAsync(
                DelayedToString,
                errors => throw new Exception("This should not be executed"));

            // Assert
            val.ValueOrDefault(null).Should().BeNull();
            val.ErrorsOrEmpty().Should().BeEquivalentTo("error");

            async Task<Result<string>> DelayedToString(int value)
            {
                await Task.Delay(1);
                return Result.Failure<string>("error");
            }
        }

        [TestMethod]
        public async Task Match_Value_Async()
        {
            var result = Result.Success(5);

            var sideEffect = false;

            await result.MatchAsync(
                async value =>
                {
                    await Task.Delay(1); // simultate some work
                    sideEffect = true;
                },
                errors => throw new Exception("This should not be executed"));

            sideEffect.Should().BeTrue();
        }

        [TestMethod]
        public async Task Convert_Value_Async()
        {
            var result = Result.Success(5);

            var value = await result.ValueAsync(
                async value =>
                {
                    await Task.Delay(1); // simultate some work
                    return "something";
                },
                errors => throw new Exception("This should not be executed"));

            value.Should().Be("something");
        }
    }
}

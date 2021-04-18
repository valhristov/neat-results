using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
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

        [TestMethod]
        public async Task Convert_Failure_To_Success_Async()
        {
            // Arrange
            var result = Result.Failure<int>("error");

            // Act
            var val = await result.SelectAsync(
                value => throw new Exception("This should not be executed"),
                OnErrorAsync);

            // Assert
            val.ValueOrThrow().Should().Be("5");
            val.ErrorsOrEmpty().Should().BeEmpty();

            async Task<Result<string>> OnErrorAsync(ImmutableArray<string> errors)
            {
                await Task.Delay(1);
                return Result.Success("5");
            }
        }

        [TestMethod]
        public async Task Convert_Failure_To_Another_Failure_Async()
        {
            // Arrange
            var result = Result.Failure<int>("error");

            // Act
            var val = await result.SelectAsync(
                value => throw new Exception("This should not be executed"),
                OnErrorAsync);

            // Assert
            val.ValueOrDefault(null).Should().Be(null);
            val.ErrorsOrEmpty().Should().BeEquivalentTo("error", "another error");

            async Task<Result<string>> OnErrorAsync(ImmutableArray<string> errors)
            {
                await Task.Delay(1);
                return Result.Failure<string>(errors.Add("another error"));
            }
        }

        [TestMethod]
        public async Task Match_Value_Async()
        {
            var result = Result.Failure<int>("error");

            var sideEffect = false;

            await result.MatchAsync(
                value => throw new Exception("This should not be executed"),
                async errors =>
                {
                    await Task.Delay(1); // simultate some work
                    sideEffect = true;
                });

            sideEffect.Should().BeTrue();
        }
    }
}

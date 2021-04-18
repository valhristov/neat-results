using System.Collections.Immutable;

namespace Neat.Results
{
    public static class Result
    {
        /// <summary>
        /// Creates a failure result from errors.
        /// </summary>
        public static Result<T> Failure<T>(params string[] errors) =>
            new Result<T>.Failure(ImmutableArray.Create(errors));

        /// <summary>
        /// Creates a failure result from errors.
        /// </summary>
        public static Result<T> Failure<T>(ImmutableArray<string> errors) =>
            new Result<T>.Failure(errors);

        /// <summary>
        /// Creates a successful result from value.
        /// </summary>
        public static Result<T> Success<T>(T value) =>
            new Result<T>.Success(value);
    }
}

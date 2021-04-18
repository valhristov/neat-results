using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Neat.Results
{
    public class Result<T>
    {
        public T ValueOrThrow() =>
            ValueOrThrow(() => new InvalidOperationException("Failure does not have a value"));

        public T ValueOrThrow<TException>(Func<TException> factory) where TException : Exception =>
            Value(value => value, _ => throw factory.Invoke());

        public T ValueOrDefault(T @default) =>
            Value(value => value, _ => @default);

        public ImmutableArray<string> ErrorsOrEmpty() =>
            Value(value => ImmutableArray<string>.Empty, errors => errors);

        public TOut Value<TOut>(Func<T, TOut> onSuccess, Func<ImmutableArray<string>, TOut> onFailure) =>
            this switch
            {
                Success success => onSuccess(success.Value),
                Failure failure => onFailure(failure.Errors),
                _ => throw new NotSupportedException(),
            };

        public Result<TOut> Select<TOut>(Func<T, Result<TOut>> onSuccess, Func<ImmutableArray<string>, Result<TOut>> onFailure = null) =>
            this switch
            {
                Success success => onSuccess(success.Value),
                Failure failure => onFailure != null ? onFailure(failure.Errors) : Result.Failure<TOut>(failure.Errors),
                _ => throw new NotSupportedException(),
            };

        public void Match(Action<T> onSuccess, Action<ImmutableArray<string>> onFailure)
        {
            switch (this)
            {
                case Success success: onSuccess(success.Value); break;
                case Failure failure: onFailure(failure.Errors); break;
                default: throw new NotSupportedException();
            }
        }

        public Task<Result<TOut>> SelectAsync<TOut>(Func<T, Task<Result<TOut>>> onSuccess, Func<ImmutableArray<string>, Task<Result<TOut>>> onFailure = null) =>
            this switch
            {
                Success success => onSuccess(success.Value),
                Failure failure =>
                    onFailure != null
                        ? onFailure(failure.Errors)
                        : Task.FromResult(Result.Failure<TOut>(failure.Errors)),
                _ => throw new NotSupportedException(),
            };

        public Task MatchAsync(Func<T, Task> onSuccess, Func<ImmutableArray<string>, Task> onFailure) =>
            this switch
            {
                Success success => onSuccess(success.Value),
                Failure failure => onFailure(failure.Errors),
                _ => throw new NotSupportedException(),
            };

        private Result() { }

        internal sealed class Success : Result<T>
        {
            public T Value { get; }

            public Success(T value)
            {
                Value = value;
            }
        }

        internal sealed class Failure : Result<T>
        {
            public ImmutableArray<string> Errors { get; }

            public Failure(ImmutableArray<string> errors)
            {
                Errors = errors;
            }
        }
    }
}

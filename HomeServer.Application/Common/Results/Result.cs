using System.Diagnostics.CodeAnalysis;

namespace HomeServer.Application.Common.Results
{
    public static class Result
    {
        public static Result<T> Success<T>(T value) where T : class
            => Result<T>.Success(value);

        public static FailureResult Failure(string error)
            => new(error);
    }

    public readonly record struct FailureResult(string Error);

    public class Result<T> where T : class
    {
        private readonly T? _value;
        private readonly string? _error;

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public T Value =>
            IsSuccess
                ? _value!
                : throw new InvalidOperationException("Result is an error.");

        public string Error =>
            IsFailure
                ? _error!
                : throw new InvalidOperationException("Result is successful.");

        private Result(T value)
        {
            IsSuccess = true;
            _value = value;
            _error = null;
        }

        private Result(string error)
        {
            IsSuccess = false;
            _value = null;
            _error = error;
        }

        public static Result<T> Success(T value)
            => new(value);

        public static Result<T> Failure(string error)
            => new(error);

        public static implicit operator Result<T>(FailureResult failure)
            => new(failure.Error);

        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsOk()
        {
            return IsSuccess && _value is not null;
        }
    }
}
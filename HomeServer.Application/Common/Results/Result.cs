using System.Diagnostics.CodeAnalysis;

namespace HomeServer.Application.Common.Results
{
    public static class Result
    {
        public static Result<T> Success<T>(T value) where T : class
            => Result<T>.Success(value);

        public static FailureResult Failure(
            string error,
            int? errorCode = null,
            string? errorTitle = null)
            => new(
                error,
                errorCode ?? 400,
                errorTitle ?? "BadRequest"
            );

        public static FailureResult Failure(
            string error,
            ProblemType problem)
            => new(
                error,
                problem.GetStatusCode(),
                problem.GetTitle()
            );
    }

    public readonly record struct FailureResult(
        string Error,
        int ErrorCode = 400,
        string ErrorTitle = "BadRequest"
    );

    public class Result<T> where T : class
    {
        private readonly T? _value;
        private readonly string? _error;
        private readonly string? _errorTitle;
        private readonly int? _errorCode;

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

        public int ErrorCode =>
            IsFailure
                ? (_errorCode ?? 400)
                : throw new InvalidOperationException("Result is successful.");

        public string ErrorTitle =>
            IsFailure
                ? (_errorTitle ?? "BadRequest")
                : throw new InvalidOperationException("Result is successful.");

        private Result(T value)
        {
            IsSuccess = true;
            _value = value;
            _error = null;
            _errorCode = null;
            _errorTitle = null;
        }

        private Result(
            string error,
            int errorCode = 400,
            string errorTitle = "BadRequest")
        {
            IsSuccess = false;
            _value = null;
            _error = error;
            _errorCode = errorCode;
            _errorTitle = errorTitle;
        }

        public static Result<T> Success(T value)
            => new(value);

        public static Result<T> Failure(
            string error,
            int errorCode = 400,
            string errorTitle = "BadRequest")
            => new(error, errorCode, errorTitle);

        public static implicit operator Result<T>(FailureResult failure)
            => new(
                failure.Error,
                failure.ErrorCode,
                failure.ErrorTitle
            );

        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsOk()
        {
            return IsSuccess && _value is not null;
        }
    }
}
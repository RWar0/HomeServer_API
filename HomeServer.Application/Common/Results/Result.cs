using System.Diagnostics.CodeAnalysis;

namespace HomeServer.Application.Common.Results
{
    public class Result<T> where T : class
    {
        private readonly T? _value;
        private readonly string? _error;

        public bool IsSuccess { get; }

        public T Value =>
            IsSuccess
                ? _value!
                : throw new InvalidOperationException("Result is an error.");

        public string Error =>
            !IsSuccess
                ? _error!
                : throw new InvalidOperationException("Result is successful.");

        private Result(bool success, T? value, string? error)
        {
            IsSuccess = success;
            _value = value;
            _error = error;
        }

        public static Result<T> Success(T value)
            => new(true, value, null);

        public static Result<T> Failure(string error)
            => new(false, null, error);

        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsOk()
        {
            return IsSuccess && Value != null;
        }
    }
}

using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Common;

public sealed class Result<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public IReadOnlyList<ValidationError> Errors { get; }

    private Result(bool isSuccess, T? value, IReadOnlyList<ValidationError> validationErrors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = validationErrors ?? [];
    }

    public static Result<T> Success(T value) => new(true, value, []);

    public static Result<T> Failure(IReadOnlyList<ValidationError> validationErrors) => new(false, default, validationErrors);

}
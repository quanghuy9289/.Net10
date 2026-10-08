using System.Diagnostics.CodeAnalysis;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Common;

public sealed class Result<T> where T : notnull
{
    private readonly T _value;

    public bool IsSuccess { get; }
    public T Value => IsSuccess 
        ? _value 
        : throw new InvalidOperationException("Cannot access Value when the result is a failure.");
    public IReadOnlyList<ValidationError> Errors { get; }

    private Result(bool isSuccess, T value, IReadOnlyList<ValidationError> validationErrors)
    {
        IsSuccess = isSuccess;
        _value = value;
        Errors = [..validationErrors];
    }

    public static Result<T> Success(T value) => new(true, value, []);

    public static Result<T> Failure(IReadOnlyList<ValidationError> validationErrors) => new(false, default!, validationErrors);

}
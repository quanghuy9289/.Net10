using MiniCrm.Domain.Common;
using MiniCrm.Domain.Validation;

namespace MiniCrm.Domain.Tests.Common;

public class ResultTest
{
    [Fact]
    public void Success_returns_a_successful_result()
    {
        var result = Result<int>.Success(1);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Failure_returns_a_failed_result_with_the_error()
    {
        IReadOnlyList<ValidationError> validationErrors = new List<ValidationError>
        {
            new("Field1", "Error message 1"),
            new("Field2", "Error message 2")
        };
        var result = Result<int>.Failure(validationErrors);

        Assert.False(result.IsSuccess);
        Assert.Equal(validationErrors.Count, result.Errors.Count);
    }
}

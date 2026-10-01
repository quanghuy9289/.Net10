namespace MiniCrm.Domain.Validation;

public record ValidationError(string Field, string Message);
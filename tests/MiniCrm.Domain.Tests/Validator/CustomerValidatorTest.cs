using MiniCrm.Domain.Validation;
using Shouldly;

namespace MiniCrm.Domain.Tests.Validator;

public class CustomerValidatorTest
{
    [Fact]
    public void Should_Have_Error_When_Name_Is_Empty()
    {
        var customer = ValidCustomer(name: string.Empty);
        var validator = new CustomerValidator();

        validator.ValidateCustomer(customer).Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Should_Have_Error_When_Name_Is_Too_Long()
    {
        var customer = ValidCustomer(name: new string('a', 256));
        var validator = new CustomerValidator();

        validator.ValidateCustomer(customer).Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Should_Have_Error_When_Email_Has_Wrong_Format()
    {
        var customer = ValidCustomer(email: "not-an-email");
        var validator = new CustomerValidator();

        validator.ValidateCustomer(customer).Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void Should_Not_Have_Errors_When_All_Fields_Are_Valid()
    {
        var validator = new CustomerValidator();
        var result = validator.ValidateCustomer(ValidCustomer());

        result.Count.ShouldBe(0);
    }

    [Fact]
    public void Should_Return_Multiple_Errors_When_Many_Fields_Are_Invalid()
    {
        var customer = ValidCustomer(
            name: string.Empty,
            email: "not-an-email");
        var validator = new CustomerValidator();

        var result = validator.ValidateCustomer(customer);

        result.Any(error => error.Message.Contains("Customer name is required")).ShouldBeTrue();
        result.Any(error => error.Message.Contains("Invalid email format")).ShouldBeTrue();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    private static CustomerRequest ValidCustomer(
        string name = "Alex Smith",
        string email = "alex@example.com",
        DateTimeOffset? createdAt = null) => new()
        {
            Name = name,
            Email = email,
        };

}
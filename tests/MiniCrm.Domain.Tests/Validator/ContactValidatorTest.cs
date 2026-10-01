using MiniCrm.Domain.Entities;
using MiniCrm.Domain.Validation;
using Shouldly;

namespace MiniCrm.Domain.Tests.Validator;

public class ContactValidatorTest
{
    private readonly ContactValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var contact = new Contact
        {
            Type = ContactType.Email,
            Value = "huy@example.com@123",
        };

        var result = _validator.Validate(contact);
        result.First().Message.ShouldBe("Contact value must be a valid email address.");
    }

    [Fact]
    public void Should_Have_Error_When_Phone_Is_Invalid()
    {
        var contact = new Contact
        {
            Type = ContactType.Phone,
            Value = "abc",
        };

        var result = _validator.Validate(contact);

        var error = result.ShouldHaveSingleItem();

        error.Field.ShouldBe("Phone");
        error.Message.ShouldBe("Contact value must be a valid phone number.");
    }

    [Fact]
    public void Should_Have_Error_When_Address_Is_Invalid()
    {
        var contact = new Contact
        {
            Type = ContactType.Address,
            Value = "",
        };

        var result = _validator.Validate(contact);

        var error = result.ShouldHaveSingleItem();

        error.Field.ShouldBe("Address");
        error.Message.ShouldBe("Contact value must be a valid address.");
    }

    [Fact]
    public void Should_Have_Error_When_Other_Type_Is_Invalid()
    {
        var contact = new Contact
        {
            Type = (ContactType)99,
            Value = "abc",
        };

        var result = _validator.Validate(contact);

        var error = result.ShouldHaveSingleItem();

        error.Field.ShouldBe("Other");
        error.Message.ShouldBe("Contact type is not supported.");
    }

    [Fact]
    public void Should_Have_Error_When_Unknown_Type()
    {
        var contact = new Contact
        {
            Type = ContactType.Unknown,
            Value = "abc",
        };

        var result = _validator.Validate(contact);

        var error = result.ShouldHaveSingleItem();

        error.Field.ShouldBe("Unknown");
        error.Message.ShouldBe("Contact type must be specified.");
    }


    [Fact]
    public void Should_Pass_When_Contact_Is_Valid()
    {
        var contact = new Contact
        {
            Type = ContactType.Phone,
            Value = "0909123456"
        };

        var result = _validator.Validate(contact);

        result.ShouldBeEmpty();
    }
}

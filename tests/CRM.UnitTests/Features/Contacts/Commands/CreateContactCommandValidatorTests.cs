namespace CRM.UnitTests.Features.Contacts.Commands;

using CRM.Application.Features.Contacts.Commands;
using FluentAssertions;
using FluentValidation.TestHelper;

public class CreateContactCommandValidatorTests
{
    private readonly CreateContactCommandValidator _sut = new();

    [Fact]
    public void Validate_ValidCommand_PassesValidation()
    {
        var command = new CreateContactCommand("Alice", "Smith", "alice@example.com", null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyFirstName_FailsWithMessage()
    {
        var command = new CreateContactCommand("", "Smith", "alice@example.com", null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
              .WithErrorMessage("First name is required.");
    }

    [Fact]
    public void Validate_EmptyLastName_FailsWithMessage()
    {
        var command = new CreateContactCommand("Alice", "", "alice@example.com", null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.LastName)
              .WithErrorMessage("Last name is required.");
    }

    [Fact]
    public void Validate_EmptyEmail_FailsWithMessage()
    {
        var command = new CreateContactCommand("Alice", "Smith", "", null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("Email is required.");
    }

    [Fact]
    public void Validate_InvalidEmailFormat_FailsWithMessage()
    {
        var command = new CreateContactCommand("Alice", "Smith", "not-an-email", null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email)
              .WithErrorMessage("A valid email is required.");
    }

    [Fact]
    public void Validate_EmailExceedsMaxLength_FailsValidation()
    {
        var longEmail = new string('a', 196) + "@b.co"; // 201 chars
        var command = new CreateContactCommand("Alice", "Smith", longEmail, null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Validate_PhoneExceedsMaxLength_FailsValidation()
    {
        var command = new CreateContactCommand("Alice", "Smith", "alice@example.com", new string('1', 21), null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeFalse();
        result.ShouldHaveValidationErrorFor(x => x.Phone);
    }

    [Fact]
    public void Validate_NullOptionalFields_PassesValidation()
    {
        var command = new CreateContactCommand("Alice", "Smith", "alice@example.com", null, null, null);

        var result = _sut.TestValidate(command);

        result.IsValid.Should().BeTrue();
        result.ShouldNotHaveValidationErrorFor(x => x.Phone);
        result.ShouldNotHaveValidationErrorFor(x => x.Notes);
        result.ShouldNotHaveValidationErrorFor(x => x.CompanyId);
    }
}

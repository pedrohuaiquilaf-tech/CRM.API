namespace CRM.Application.Features.Activities.Commands;

using CRM.Domain.Enums;
using FluentValidation;

public class CreateActivityCommandValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityCommandValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Activity type is required.")
            .Must(BeAValidType).WithMessage("Invalid activity type. Must be Call, Email, or Meeting.");

        RuleFor(x => x.Subject)
            .NotEmpty().WithMessage("Subject is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.ActivityDate)
            .NotEmpty().WithMessage("Activity date is required.");
    }

    private bool BeAValidType(string type)
    {
        return Enum.TryParse<ActivityType>(type, ignoreCase: true, out _);
    }
}

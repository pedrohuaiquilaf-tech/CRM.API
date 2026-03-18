namespace CRM.Application.Features.Companies.Commands;

using FluentValidation;

public class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Industry)
            .MaximumLength(100);

        RuleFor(x => x.Size)
            .MaximumLength(50);

        RuleFor(x => x.Website)
            .MaximumLength(300);
    }
}

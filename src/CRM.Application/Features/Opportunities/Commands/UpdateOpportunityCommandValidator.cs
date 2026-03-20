namespace CRM.Application.Features.Opportunities.Commands;

using CRM.Domain.Enums;
using FluentValidation;

public class UpdateOpportunityCommandValidator : AbstractValidator<UpdateOpportunityCommand>
{
    public UpdateOpportunityCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Opportunity ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0).WithMessage("Amount must be zero or positive.");

        RuleFor(x => x.Stage)
            .NotEmpty().WithMessage("Stage is required.")
            .Must(BeAValidStage).WithMessage("Invalid pipeline stage.");

        RuleFor(x => x.Probability)
            .InclusiveBetween(0, 100).WithMessage("Probability must be between 0 and 100.");
    }

    private bool BeAValidStage(string stage)
    {
        return Enum.TryParse<PipelineStage>(stage, ignoreCase: true, out _);
    }
}

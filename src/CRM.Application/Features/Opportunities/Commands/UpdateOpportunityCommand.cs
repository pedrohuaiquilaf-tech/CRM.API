namespace CRM.Application.Features.Opportunities.Commands;

using MediatR;

public record UpdateOpportunityCommand(
    Guid Id,
    string Title,
    decimal Amount,
    string Stage,
    double Probability,
    DateTime? ExpectedCloseDate,
    Guid? ContactId,
    Guid? CompanyId
) : IRequest<bool>;

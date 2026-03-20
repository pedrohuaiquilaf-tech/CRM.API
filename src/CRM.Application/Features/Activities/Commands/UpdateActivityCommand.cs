namespace CRM.Application.Features.Activities.Commands;

using MediatR;

public record UpdateActivityCommand(
    Guid Id,
    string Type,
    string Subject,
    string? Description,
    DateTime ActivityDate,
    Guid? ContactId,
    Guid? OpportunityId
) : IRequest<bool>;

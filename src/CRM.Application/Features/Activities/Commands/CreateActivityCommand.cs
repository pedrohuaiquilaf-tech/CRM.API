namespace CRM.Application.Features.Activities.Commands;

using MediatR;

public record CreateActivityCommand(
    string Type,
    string Subject,
    string? Description,
    DateTime ActivityDate,
    Guid? ContactId,
    Guid? OpportunityId
) : IRequest<Guid>;

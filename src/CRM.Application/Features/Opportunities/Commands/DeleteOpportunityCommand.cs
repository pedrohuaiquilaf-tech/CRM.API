namespace CRM.Application.Features.Opportunities.Commands;

using MediatR;

public record DeleteOpportunityCommand(Guid Id) : IRequest<bool>;

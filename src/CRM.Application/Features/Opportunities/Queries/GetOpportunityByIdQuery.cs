namespace CRM.Application.Features.Opportunities.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetOpportunityByIdQuery(Guid Id) : IRequest<OpportunityDto?>;

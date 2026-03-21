namespace CRM.Application.Features.Opportunities.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetAllOpportunitiesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = null,
    string? Search = null
) : IRequest<PaginatedResult<OpportunityDto>>;

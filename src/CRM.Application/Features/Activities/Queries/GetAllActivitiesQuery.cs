namespace CRM.Application.Features.Activities.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetAllActivitiesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = null,
    string? Search = null
) : IRequest<PaginatedResult<ActivityDto>>;

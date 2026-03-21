namespace CRM.Application.Features.Companies.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetAllCompaniesQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = null,
    string? Search = null
) : IRequest<PaginatedResult<CompanyDto>>;

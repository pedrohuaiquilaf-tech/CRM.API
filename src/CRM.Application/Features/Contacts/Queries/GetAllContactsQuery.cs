namespace CRM.Application.Features.Contacts.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetAllContactsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = null,
    string? Search = null
) : IRequest<PaginatedResult<ContactDto>>;

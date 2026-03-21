namespace CRM.Application.Features.Contacts.Queries;

using System.Linq.Expressions;
using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetAllContactsQueryHandler : IRequestHandler<GetAllContactsQuery, PaginatedResult<ContactDto>>
{
    private readonly IRepository<Contact> _contactRepository;

    public GetAllContactsQueryHandler(IRepository<Contact> contactRepository)
    {
        _contactRepository = contactRepository;
    }

    public async Task<PaginatedResult<ContactDto>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Contact, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            filter = c => c.FirstName.ToLower().Contains(search)
                       || c.LastName.ToLower().Contains(search)
                       || c.Email.ToLower().Contains(search);
        }

        bool desc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        Func<IQueryable<Contact>, IOrderedQueryable<Contact>> orderBy = request.SortBy?.ToLower() switch
        {
            "firstname"  => q => desc ? q.OrderByDescending(c => c.FirstName)  : q.OrderBy(c => c.FirstName),
            "lastname"   => q => desc ? q.OrderByDescending(c => c.LastName)   : q.OrderBy(c => c.LastName),
            "email"      => q => desc ? q.OrderByDescending(c => c.Email)      : q.OrderBy(c => c.Email),
            _            => q => desc ? q.OrderByDescending(c => c.CreatedAt)  : q.OrderBy(c => c.CreatedAt),
        };

        int skip = (request.Page - 1) * request.PageSize;
        var totalCount = await _contactRepository.CountAsync(filter, cancellationToken);
        var items = await _contactRepository.GetPagedAsync(skip, request.PageSize, filter, orderBy, cancellationToken);

        var dtos = items.Select(c => new ContactDto(
            c.Id, c.FirstName, c.LastName, c.Email, c.Phone, c.Notes,
            c.CompanyId, c.Company?.Name, c.CreatedAt
        )).ToList();

        return new PaginatedResult<ContactDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}

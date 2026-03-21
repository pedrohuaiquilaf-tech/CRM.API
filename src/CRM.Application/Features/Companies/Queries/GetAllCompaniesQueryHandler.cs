namespace CRM.Application.Features.Companies.Queries;

using System.Linq.Expressions;
using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetAllCompaniesQueryHandler : IRequestHandler<GetAllCompaniesQuery, PaginatedResult<CompanyDto>>
{
    private readonly IRepository<Company> _companyRepository;

    public GetAllCompaniesQueryHandler(IRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<PaginatedResult<CompanyDto>> Handle(GetAllCompaniesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Company, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            filter = c => c.Name.ToLower().Contains(search)
                       || (c.Industry != null && c.Industry.ToLower().Contains(search));
        }

        bool desc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        Func<IQueryable<Company>, IOrderedQueryable<Company>> orderBy = request.SortBy?.ToLower() switch
        {
            "name"     => q => desc ? q.OrderByDescending(c => c.Name)      : q.OrderBy(c => c.Name),
            "industry" => q => desc ? q.OrderByDescending(c => c.Industry)  : q.OrderBy(c => c.Industry),
            _          => q => desc ? q.OrderByDescending(c => c.CreatedAt) : q.OrderBy(c => c.CreatedAt),
        };

        int skip = (request.Page - 1) * request.PageSize;
        var totalCount = await _companyRepository.CountAsync(filter, cancellationToken);
        var items = await _companyRepository.GetPagedAsync(skip, request.PageSize, filter, orderBy, cancellationToken);

        var dtos = items.Select(c => new CompanyDto(
            c.Id, c.Name, c.Industry, c.Size, c.Website,
            c.Contacts.Count, c.Opportunities.Count, c.CreatedAt
        )).ToList();

        return new PaginatedResult<CompanyDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}

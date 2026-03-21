namespace CRM.Application.Features.Opportunities.Queries;

using System.Linq.Expressions;
using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetAllOpportunitiesQueryHandler : IRequestHandler<GetAllOpportunitiesQuery, PaginatedResult<OpportunityDto>>
{
    private readonly IRepository<Opportunity> _opportunityRepository;

    public GetAllOpportunitiesQueryHandler(IRepository<Opportunity> opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<PaginatedResult<OpportunityDto>> Handle(GetAllOpportunitiesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Opportunity, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            filter = o => o.Title.ToLower().Contains(search);
        }

        bool desc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        Func<IQueryable<Opportunity>, IOrderedQueryable<Opportunity>> orderBy = request.SortBy?.ToLower() switch
        {
            "title"       => q => desc ? q.OrderByDescending(o => o.Title)               : q.OrderBy(o => o.Title),
            "amount"      => q => desc ? q.OrderByDescending(o => o.Amount)              : q.OrderBy(o => o.Amount),
            "stage"       => q => desc ? q.OrderByDescending(o => o.Stage)               : q.OrderBy(o => o.Stage),
            "probability" => q => desc ? q.OrderByDescending(o => o.Probability)         : q.OrderBy(o => o.Probability),
            _             => q => desc ? q.OrderByDescending(o => o.CreatedAt)           : q.OrderBy(o => o.CreatedAt),
        };

        int skip = (request.Page - 1) * request.PageSize;
        var totalCount = await _opportunityRepository.CountAsync(filter, cancellationToken);
        var items = await _opportunityRepository.GetPagedAsync(skip, request.PageSize, filter, orderBy, cancellationToken);

        var dtos = items.Select(o => new OpportunityDto(
            o.Id, o.Title, o.Amount, o.Stage.ToString(), o.Probability,
            o.ExpectedCloseDate, o.ContactId,
            o.Contact == null ? null : $"{o.Contact.FirstName} {o.Contact.LastName}",
            o.CompanyId, o.Company?.Name, o.CreatedAt
        )).ToList();

        return new PaginatedResult<OpportunityDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}

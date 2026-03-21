namespace CRM.Application.Features.Activities.Queries;

using System.Linq.Expressions;
using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetAllActivitiesQueryHandler : IRequestHandler<GetAllActivitiesQuery, PaginatedResult<ActivityDto>>
{
    private readonly IRepository<Activity> _activityRepository;

    public GetAllActivitiesQueryHandler(IRepository<Activity> activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<PaginatedResult<ActivityDto>> Handle(GetAllActivitiesQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Activity, bool>>? filter = null;
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            filter = a => a.Subject.ToLower().Contains(search)
                       || a.Type.ToString().ToLower().Contains(search);
        }

        bool desc = string.Equals(request.SortOrder, "desc", StringComparison.OrdinalIgnoreCase);
        Func<IQueryable<Activity>, IOrderedQueryable<Activity>> orderBy = request.SortBy?.ToLower() switch
        {
            "subject"      => q => desc ? q.OrderByDescending(a => a.Subject)      : q.OrderBy(a => a.Subject),
            "type"         => q => desc ? q.OrderByDescending(a => a.Type)         : q.OrderBy(a => a.Type),
            "activitydate" => q => desc ? q.OrderByDescending(a => a.ActivityDate) : q.OrderBy(a => a.ActivityDate),
            _              => q => desc ? q.OrderByDescending(a => a.CreatedAt)    : q.OrderBy(a => a.CreatedAt),
        };

        int skip = (request.Page - 1) * request.PageSize;
        var totalCount = await _activityRepository.CountAsync(filter, cancellationToken);
        var items = await _activityRepository.GetPagedAsync(skip, request.PageSize, filter, orderBy, cancellationToken);

        var dtos = items.Select(a => new ActivityDto(
            a.Id, a.Type.ToString(), a.Subject, a.Description, a.ActivityDate,
            a.ContactId, a.Contact == null ? null : $"{a.Contact.FirstName} {a.Contact.LastName}",
            a.OpportunityId, a.Opportunity?.Title, a.CreatedAt
        )).ToList();

        return new PaginatedResult<ActivityDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}

namespace CRM.Application.Features.Activities.Queries;

using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetActivityByIdQueryHandler : IRequestHandler<GetActivityByIdQuery, ActivityDto?>
{
    private readonly IRepository<Activity> _activityRepository;

    public GetActivityByIdQueryHandler(IRepository<Activity> activityRepository)
    {
        _activityRepository = activityRepository;
    }

    public async Task<ActivityDto?> Handle(GetActivityByIdQuery request, CancellationToken cancellationToken)
    {
        var activity = await _activityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (activity is null)
            return null;

        return new ActivityDto(
            activity.Id,
            activity.Type.ToString(),
            activity.Subject,
            activity.Description,
            activity.ActivityDate,
            activity.ContactId,
            activity.Contact?.FirstName + " " + activity.Contact?.LastName,
            activity.OpportunityId,
            activity.Opportunity?.Title,
            activity.CreatedAt
        );
    }
}

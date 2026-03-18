namespace CRM.Application.Features.Activities.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using MediatR;

public class CreateActivityCommandHandler : IRequestHandler<CreateActivityCommand, Guid>
{
    private readonly IRepository<Activity> _activityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateActivityCommandHandler(IRepository<Activity> activityRepository, IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = new Activity
        {
            Id = Guid.NewGuid(),
            Type = Enum.Parse<ActivityType>(request.Type, ignoreCase: true),
            Subject = request.Subject,
            Description = request.Description,
            ActivityDate = request.ActivityDate,
            ContactId = request.ContactId,
            OpportunityId = request.OpportunityId,
            CreatedAt = DateTime.UtcNow
        };

        await _activityRepository.AddAsync(activity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return activity.Id;
    }
}

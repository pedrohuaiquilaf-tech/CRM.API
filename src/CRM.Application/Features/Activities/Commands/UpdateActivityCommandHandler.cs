namespace CRM.Application.Features.Activities.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using MediatR;

public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, bool>
{
    private readonly IRepository<Activity> _activityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateActivityCommandHandler(IRepository<Activity> activityRepository, IUnitOfWork unitOfWork)
    {
        _activityRepository = activityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        var activity = await _activityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (activity is null)
            return false;

        activity.Type = Enum.Parse<ActivityType>(request.Type, ignoreCase: true);
        activity.Subject = request.Subject;
        activity.Description = request.Description;
        activity.ActivityDate = request.ActivityDate;
        activity.ContactId = request.ContactId;
        activity.OpportunityId = request.OpportunityId;

        await _activityRepository.UpdateAsync(activity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

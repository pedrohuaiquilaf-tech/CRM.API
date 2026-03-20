namespace CRM.Application.Features.Opportunities.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using MediatR;

public class UpdateOpportunityCommandHandler : IRequestHandler<UpdateOpportunityCommand, bool>
{
    private readonly IRepository<Opportunity> _opportunityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOpportunityCommandHandler(IRepository<Opportunity> opportunityRepository, IUnitOfWork unitOfWork)
    {
        _opportunityRepository = opportunityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (opportunity is null)
            return false;

        opportunity.Title = request.Title;
        opportunity.Amount = request.Amount;
        opportunity.Stage = Enum.Parse<PipelineStage>(request.Stage, ignoreCase: true);
        opportunity.Probability = request.Probability;
        opportunity.ExpectedCloseDate = request.ExpectedCloseDate;
        opportunity.ContactId = request.ContactId;
        opportunity.CompanyId = request.CompanyId;

        await _opportunityRepository.UpdateAsync(opportunity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

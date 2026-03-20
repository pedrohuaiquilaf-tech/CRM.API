namespace CRM.Application.Features.Opportunities.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class DeleteOpportunityCommandHandler : IRequestHandler<DeleteOpportunityCommand, bool>
{
    private readonly IRepository<Opportunity> _opportunityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOpportunityCommandHandler(IRepository<Opportunity> opportunityRepository, IUnitOfWork unitOfWork)
    {
        _opportunityRepository = opportunityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (opportunity is null)
            return false;

        await _opportunityRepository.DeleteAsync(opportunity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

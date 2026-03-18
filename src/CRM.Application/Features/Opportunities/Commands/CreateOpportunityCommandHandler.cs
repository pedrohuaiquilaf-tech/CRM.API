namespace CRM.Application.Features.Opportunities.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Enums;
using CRM.Domain.Interfaces;
using MediatR;

public class CreateOpportunityCommandHandler : IRequestHandler<CreateOpportunityCommand, Guid>
{
    private readonly IRepository<Opportunity> _opportunityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOpportunityCommandHandler(IRepository<Opportunity> opportunityRepository, IUnitOfWork unitOfWork)
    {
        _opportunityRepository = opportunityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateOpportunityCommand request, CancellationToken cancellationToken)
    {
        var opportunity = new Opportunity
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Amount = request.Amount,
            Stage = Enum.Parse<PipelineStage>(request.Stage, ignoreCase: true),
            Probability = request.Probability,
            ExpectedCloseDate = request.ExpectedCloseDate,
            ContactId = request.ContactId,
            CompanyId = request.CompanyId,
            CreatedAt = DateTime.UtcNow
        };

        await _opportunityRepository.AddAsync(opportunity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return opportunity.Id;
    }
}

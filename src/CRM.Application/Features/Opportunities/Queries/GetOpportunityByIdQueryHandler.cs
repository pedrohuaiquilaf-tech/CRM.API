namespace CRM.Application.Features.Opportunities.Queries;

using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetOpportunityByIdQueryHandler : IRequestHandler<GetOpportunityByIdQuery, OpportunityDto?>
{
    private readonly IRepository<Opportunity> _opportunityRepository;

    public GetOpportunityByIdQueryHandler(IRepository<Opportunity> opportunityRepository)
    {
        _opportunityRepository = opportunityRepository;
    }

    public async Task<OpportunityDto?> Handle(GetOpportunityByIdQuery request, CancellationToken cancellationToken)
    {
        var opp = await _opportunityRepository.GetByIdAsync(request.Id, cancellationToken);

        if (opp is null)
            return null;

        return new OpportunityDto(
            opp.Id,
            opp.Title,
            opp.Amount,
            opp.Stage.ToString(),
            opp.Probability,
            opp.ExpectedCloseDate,
            opp.ContactId,
            opp.Contact?.FirstName + " " + opp.Contact?.LastName,
            opp.CompanyId,
            opp.Company?.Name,
            opp.CreatedAt
        );
    }
}

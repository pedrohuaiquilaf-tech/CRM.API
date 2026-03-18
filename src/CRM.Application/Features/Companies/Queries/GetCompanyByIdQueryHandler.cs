namespace CRM.Application.Features.Companies.Queries;

using CRM.Application.DTOs;
using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class GetCompanyByIdQueryHandler : IRequestHandler<GetCompanyByIdQuery, CompanyDto?>
{
    private readonly IRepository<Company> _companyRepository;

    public GetCompanyByIdQueryHandler(IRepository<Company> companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<CompanyDto?> Handle(GetCompanyByIdQuery request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (company is null)
            return null;

        return new CompanyDto(
            company.Id,
            company.Name,
            company.Industry,
            company.Size,
            company.Website,
            company.Contacts?.Count ?? 0,
            company.Opportunities?.Count ?? 0,
            company.CreatedAt
        );
    }
}

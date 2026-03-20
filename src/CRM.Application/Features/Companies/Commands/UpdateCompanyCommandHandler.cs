namespace CRM.Application.Features.Companies.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, bool>
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyCommandHandler(IRepository<Company> companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (company is null)
            return false;

        company.Name = request.Name;
        company.Industry = request.Industry;
        company.Size = request.Size;
        company.Website = request.Website;

        await _companyRepository.UpdateAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

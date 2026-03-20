namespace CRM.Application.Features.Companies.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class DeleteCompanyCommandHandler : IRequestHandler<DeleteCompanyCommand, bool>
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCompanyCommandHandler(IRepository<Company> companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _companyRepository.GetByIdAsync(request.Id, cancellationToken);

        if (company is null)
            return false;

        await _companyRepository.DeleteAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

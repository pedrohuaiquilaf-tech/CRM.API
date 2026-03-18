namespace CRM.Application.Features.Companies.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Guid>
{
    private readonly IRepository<Company> _companyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCommandHandler(IRepository<Company> companyRepository, IUnitOfWork unitOfWork)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = new Company
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Industry = request.Industry,
            Size = request.Size,
            Website = request.Website,
            CreatedAt = DateTime.UtcNow
        };

        await _companyRepository.AddAsync(company, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return company.Id;
    }
}

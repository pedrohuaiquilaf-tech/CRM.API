namespace CRM.Application.Features.Contacts.Commands;

using CRM.Domain.Entities;
using CRM.Domain.Interfaces;
using MediatR;

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, bool>
{
    private readonly IRepository<Contact> _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateContactCommandHandler(IRepository<Contact> contactRepository, IUnitOfWork unitOfWork)
    {
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var contact = await _contactRepository.GetByIdAsync(request.Id, cancellationToken);

        if (contact is null)
            return false;

        contact.FirstName = request.FirstName;
        contact.LastName = request.LastName;
        contact.Email = request.Email;
        contact.Phone = request.Phone;
        contact.Notes = request.Notes;
        contact.CompanyId = request.CompanyId;

        await _contactRepository.UpdateAsync(contact, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}

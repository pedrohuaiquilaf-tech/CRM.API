namespace CRM.Application.Features.Contacts.Commands;

using MediatR;

public record UpdateContactCommand(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Notes,
    Guid? CompanyId
) : IRequest<bool>;

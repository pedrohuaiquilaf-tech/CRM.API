namespace CRM.Application.Features.Contacts.Commands;

using MediatR;

public record DeleteContactCommand(Guid Id) : IRequest<bool>;

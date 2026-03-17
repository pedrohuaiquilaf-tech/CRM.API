namespace CRM.Application.Features.Contacts.Queries;

using CRM.Application.DTOs;
using MediatR;

public record GetContactByIdQuery(Guid Id) : IRequest<ContactDto?>;

namespace CRM.Application.Features.Companies.Commands;

using MediatR;

public record DeleteCompanyCommand(Guid Id) : IRequest<bool>;

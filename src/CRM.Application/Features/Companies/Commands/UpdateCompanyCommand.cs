namespace CRM.Application.Features.Companies.Commands;

using MediatR;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string? Industry,
    string? Size,
    string? Website
) : IRequest<bool>;

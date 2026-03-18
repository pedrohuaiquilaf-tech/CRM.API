namespace CRM.Application.Features.Companies.Commands;

using MediatR;

public record CreateCompanyCommand(
    string Name,
    string? Industry,
    string? Size,
    string? Website
) : IRequest<Guid>;

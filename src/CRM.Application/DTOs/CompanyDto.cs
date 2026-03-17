namespace CRM.Application.DTOs;

public record CompanyDto(
    Guid Id,
    string Name,
    string? Industry,
    string? Size,
    string? Website,
    int ContactCount,
    int OpportunityCount,
    DateTime CreatedAt
);

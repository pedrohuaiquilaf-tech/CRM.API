namespace CRM.Application.DTOs;

public record ContactDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Notes,
    Guid? CompanyId,
    string? CompanyName,
    DateTime CreatedAt
);

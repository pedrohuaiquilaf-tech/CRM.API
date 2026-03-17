namespace CRM.Application.DTOs;

public record OpportunityDto(
    Guid Id,
    string Title,
    decimal Amount,
    string Stage,
    double Probability,
    DateTime? ExpectedCloseDate,
    Guid? ContactId,
    string? ContactName,
    Guid? CompanyId,
    string? CompanyName,
    DateTime CreatedAt
);

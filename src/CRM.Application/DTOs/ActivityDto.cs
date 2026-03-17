namespace CRM.Application.DTOs;

public record ActivityDto(
    Guid Id,
    string Type,
    string Subject,
    string? Description,
    DateTime ActivityDate,
    Guid? ContactId,
    string? ContactName,
    Guid? OpportunityId,
    string? OpportunityTitle,
    DateTime CreatedAt
);

namespace CRM.Domain.Entities;

using CRM.Domain.Common;
using CRM.Domain.Enums;

public class Activity : BaseEntity
{
    public ActivityType Type { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime ActivityDate { get; set; }

    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public Guid? OpportunityId { get; set; }
    public Opportunity? Opportunity { get; set; }
}

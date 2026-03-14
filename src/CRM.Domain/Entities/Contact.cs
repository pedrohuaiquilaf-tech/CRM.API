namespace CRM.Domain.Entities;

using CRM.Domain.Common;

public class Contact : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Notes { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}

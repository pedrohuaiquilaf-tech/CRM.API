namespace CRM.Domain.Entities;

using CRM.Domain.Common;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Size { get; set; }
    public string? Website { get; set; }

    public ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public ICollection<Opportunity> Opportunities { get; set; } = new List<Opportunity>();
}

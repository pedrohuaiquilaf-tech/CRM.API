namespace CRM.Domain.Entities;

using CRM.Domain.Common;
using CRM.Domain.Enums;

public class Opportunity : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PipelineStage Stage { get; set; } = PipelineStage.Lead;
    public double Probability { get; set; }
    public DateTime? ExpectedCloseDate { get; set; }

    public Guid? ContactId { get; set; }
    public Contact? Contact { get; set; }

    public Guid? CompanyId { get; set; }
    public Company? Company { get; set; }

    public ICollection<Activity> Activities { get; set; } = new List<Activity>();
}

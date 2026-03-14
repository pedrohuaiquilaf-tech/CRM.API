namespace CRM.Infrastructure.Data.Configurations;

using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class OpportunityConfiguration : IEntityTypeConfiguration<Opportunity>
{
    public void Configure(EntityTypeBuilder<Opportunity> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Amount)
            .HasPrecision(18, 2);

        builder.Property(o => o.Probability)
            .HasDefaultValue(0);

        builder.Property(o => o.Stage)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(o => o.Contact)
            .WithMany(c => c.Opportunities)
            .HasForeignKey(o => o.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(o => o.Company)
            .WithMany(co => co.Opportunities)
            .HasForeignKey(o => o.CompanyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(o => !o.IsDeleted);
    }
}

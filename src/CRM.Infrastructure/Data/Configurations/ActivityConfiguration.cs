namespace CRM.Infrastructure.Data.Configurations;

using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Subject)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(2000);

        builder.Property(a => a.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasOne(a => a.Contact)
            .WithMany(c => c.Activities)
            .HasForeignKey(a => a.ContactId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Opportunity)
            .WithMany(o => o.Activities)
            .HasForeignKey(a => a.OpportunityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

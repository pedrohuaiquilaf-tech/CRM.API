namespace CRM.Infrastructure.Data.Configurations;

using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Industry)
            .HasMaxLength(100);

        builder.Property(c => c.Size)
            .HasMaxLength(50);

        builder.Property(c => c.Website)
            .HasMaxLength(300);

        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

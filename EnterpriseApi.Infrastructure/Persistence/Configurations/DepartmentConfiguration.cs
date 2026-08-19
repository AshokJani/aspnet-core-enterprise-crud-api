using EnterpriseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApi.Infrastructure.Persistence.Configurations;

public sealed class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");
        builder.HasKey(department => department.Id);

        builder.Property(department => department.Name)
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(department => department.Name).IsUnique();

        builder.Property(department => department.Description).HasMaxLength(500);
        builder.Property(department => department.IsActive).HasDefaultValue(true);
        builder.Property(department => department.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();
    }
}

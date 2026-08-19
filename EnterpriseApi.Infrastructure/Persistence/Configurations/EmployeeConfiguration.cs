using EnterpriseApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnterpriseApi.Infrastructure.Persistence.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");
        builder.HasKey(employee => employee.Id);

        builder.Property(employee => employee.EmployeeCode)
            .HasMaxLength(50)
            .IsRequired();
        builder.HasIndex(employee => employee.EmployeeCode).IsUnique();

        builder.Property(employee => employee.FirstName)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(employee => employee.LastName)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(employee => employee.Email)
            .HasMaxLength(256)
            .IsRequired();
        builder.HasIndex(employee => employee.Email).IsUnique();

        builder.Property(employee => employee.Salary).HasPrecision(18, 2);
        builder.Property(employee => employee.IsActive).HasDefaultValue(true);
        builder.Property(employee => employee.CreatedAtUtc)
            .HasDefaultValueSql("SYSUTCDATETIME()")
            .IsRequired();

        builder.HasOne(employee => employee.Department)
            .WithMany(department => department.Employees)
            .HasForeignKey(employee => employee.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

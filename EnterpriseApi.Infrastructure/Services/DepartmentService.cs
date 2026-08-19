using EnterpriseApi.Application.DTOs;
using EnterpriseApi.Application.Exceptions;
using EnterpriseApi.Application.Requests;
using EnterpriseApi.Application.Services;
using EnterpriseApi.Domain.Entities;
using EnterpriseApi.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApi.Infrastructure.Services;

public sealed class DepartmentService(AppDbContext dbContext) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(department => department.Id)
            .Select(department => new DepartmentDto(
                department.Id,
                department.Name,
                department.Description,
                department.IsActive,
                department.CreatedAtUtc,
                department.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .AsNoTracking()
            .Where(department => department.Id == id)
            .Select(department => new DepartmentDto(
                department.Id,
                department.Name,
                department.Description,
                department.IsActive,
                department.CreatedAtUtc,
                department.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        return department ?? throw new NotFoundException($"Department with id {id} was not found.");
    }

    public async Task<DepartmentDto> CreateAsync(
        CreateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        await EnsureUniqueNameAsync(name, null, cancellationToken);

        var department = new Department
        {
            Name = name,
            Description = NormalizeOptional(request.Description),
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Departments.Add(department);
        await SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(department.Id, cancellationToken);
    }

    public async Task UpdateAsync(
        int id,
        UpdateDepartmentRequest request,
        CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .FirstOrDefaultAsync(department => department.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Department with id {id} was not found.");

        var name = request.Name.Trim();
        await EnsureUniqueNameAsync(name, id, cancellationToken);

        department.Name = name;
        department.Description = NormalizeOptional(request.Description);
        department.IsActive = request.IsActive;
        department.UpdatedAtUtc = DateTime.UtcNow;

        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var department = await dbContext.Departments
            .FirstOrDefaultAsync(department => department.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Department with id {id} was not found.");

        if (await dbContext.Employees.AsNoTracking()
                .AnyAsync(employee => employee.DepartmentId == id, cancellationToken))
        {
            throw new ConflictException("The department cannot be deleted while it has employees.");
        }

        dbContext.Departments.Remove(department);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 547 })
        {
            throw new ConflictException("The department cannot be deleted while it has employees.", exception);
        }
    }

    private async Task EnsureUniqueNameAsync(
        string name,
        int? excludedId,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Departments.AsNoTracking().AnyAsync(
                department => department.Name == name && department.Id != excludedId,
                cancellationToken))
        {
            throw new ConflictException($"Department name '{name}' is already in use.");
        }
    }

    private async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (
            exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("A department with the same name already exists.", exception);
        }
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

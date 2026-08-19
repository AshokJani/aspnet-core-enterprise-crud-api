using EnterpriseApi.Application.DTOs;
using EnterpriseApi.Application.Exceptions;
using EnterpriseApi.Application.Requests;
using EnterpriseApi.Application.Services;
using EnterpriseApi.Domain.Entities;
using EnterpriseApi.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApi.Infrastructure.Services;

public sealed class EmployeeService(AppDbContext dbContext) : IEmployeeService
{
    public async Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Employees
            .AsNoTracking()
            .OrderBy(employee => employee.Id)
            .Select(employee => new EmployeeDto(
                employee.Id,
                employee.EmployeeCode,
                employee.FirstName,
                employee.LastName,
                employee.Email,
                employee.Salary,
                employee.DepartmentId,
                employee.Department.Name,
                employee.IsActive,
                employee.CreatedAtUtc,
                employee.UpdatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .Where(employee => employee.Id == id)
            .Select(employee => new EmployeeDto(
                employee.Id,
                employee.EmployeeCode,
                employee.FirstName,
                employee.LastName,
                employee.Email,
                employee.Salary,
                employee.DepartmentId,
                employee.Department.Name,
                employee.IsActive,
                employee.CreatedAtUtc,
                employee.UpdatedAtUtc))
            .FirstOrDefaultAsync(cancellationToken);

        return employee ?? throw new NotFoundException($"Employee with id {id} was not found.");
    }

    public async Task<EmployeeDto> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employeeCode = request.EmployeeCode.Trim();
        var email = request.Email.Trim();
        await EnsureDepartmentExistsAsync(request.DepartmentId, cancellationToken);
        await EnsureUniqueAsync(employeeCode, email, null, cancellationToken);

        var employee = new Employee
        {
            EmployeeCode = employeeCode,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            Salary = request.Salary,
            DepartmentId = request.DepartmentId,
            IsActive = request.IsActive,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Employees.Add(employee);
        await SaveChangesAsync(cancellationToken);
        return await GetByIdAsync(employee.Id, cancellationToken);
    }

    public async Task UpdateAsync(
        int id,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");

        var employeeCode = request.EmployeeCode.Trim();
        var email = request.Email.Trim();
        await EnsureDepartmentExistsAsync(request.DepartmentId, cancellationToken);
        await EnsureUniqueAsync(employeeCode, email, id, cancellationToken);

        employee.EmployeeCode = employeeCode;
        employee.FirstName = request.FirstName.Trim();
        employee.LastName = request.LastName.Trim();
        employee.Email = email;
        employee.Salary = request.Salary;
        employee.DepartmentId = request.DepartmentId;
        employee.IsActive = request.IsActive;
        employee.UpdatedAtUtc = DateTime.UtcNow;

        await SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(employee => employee.Id == id, cancellationToken)
            ?? throw new NotFoundException($"Employee with id {id} was not found.");

        dbContext.Employees.Remove(employee);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureDepartmentExistsAsync(int departmentId, CancellationToken cancellationToken)
    {
        if (!await dbContext.Departments.AsNoTracking()
                .AnyAsync(department => department.Id == departmentId, cancellationToken))
        {
            throw new RequestValidationException($"Department with id {departmentId} does not exist.");
        }
    }

    private async Task EnsureUniqueAsync(
        string employeeCode,
        string email,
        int? excludedId,
        CancellationToken cancellationToken)
    {
        if (await dbContext.Employees.AsNoTracking().AnyAsync(
                employee => employee.EmployeeCode == employeeCode && employee.Id != excludedId,
                cancellationToken))
        {
            throw new ConflictException($"EmployeeCode '{employeeCode}' is already in use.");
        }

        if (await dbContext.Employees.AsNoTracking().AnyAsync(
                employee => employee.Email == email && employee.Id != excludedId,
                cancellationToken))
        {
            throw new ConflictException($"Email '{email}' is already in use.");
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
            throw new ConflictException("An employee with the same EmployeeCode or Email already exists.", exception);
        }
    }
}

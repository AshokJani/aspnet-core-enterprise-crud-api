namespace EnterpriseApi.Application.DTOs;

public sealed record EmployeeDto(
    int Id,
    string EmployeeCode,
    string FirstName,
    string LastName,
    string Email,
    decimal Salary,
    int DepartmentId,
    string DepartmentName,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

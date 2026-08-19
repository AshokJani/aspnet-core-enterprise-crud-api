namespace EnterpriseApi.Application.DTOs;

public sealed record DepartmentDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc);

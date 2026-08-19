using System.ComponentModel.DataAnnotations;

namespace EnterpriseApi.Application.Requests;

public sealed class UpdateDepartmentRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; init; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; init; }

    public bool IsActive { get; init; } = true;
}

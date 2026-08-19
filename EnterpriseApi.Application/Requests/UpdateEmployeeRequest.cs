using System.ComponentModel.DataAnnotations;

namespace EnterpriseApi.Application.Requests;

public sealed class UpdateEmployeeRequest
{
    [Required]
    [StringLength(50)]
    public string EmployeeCode { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "9999999999999999.99")]
    public decimal Salary { get; init; }

    [Range(1, int.MaxValue)]
    public int DepartmentId { get; init; }

    public bool IsActive { get; init; }
}

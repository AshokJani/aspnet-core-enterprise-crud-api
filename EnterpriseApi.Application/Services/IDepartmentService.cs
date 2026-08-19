using EnterpriseApi.Application.DTOs;
using EnterpriseApi.Application.Requests;

namespace EnterpriseApi.Application.Services;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<DepartmentDto> CreateAsync(CreateDepartmentRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(int id, UpdateDepartmentRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

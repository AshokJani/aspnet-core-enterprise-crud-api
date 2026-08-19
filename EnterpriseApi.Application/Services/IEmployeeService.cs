using EnterpriseApi.Application.DTOs;
using EnterpriseApi.Application.Requests;

namespace EnterpriseApi.Application.Services;

public interface IEmployeeService
{
    Task<IReadOnlyList<EmployeeDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<EmployeeDto> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken);
    Task UpdateAsync(int id, UpdateEmployeeRequest request, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}

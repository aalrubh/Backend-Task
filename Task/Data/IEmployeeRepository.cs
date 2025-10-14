using MyApp.DTOs;
using MyApp.Models;

namespace MyApp.Data;

public interface IEmployeeRepository : IRepositoryBase<EmployeeModel>
{
    public Task<EmployeeDTO?> GetEmployeeByIdAsync(int id);

    public Task<List<EmployeeDTO>> GetEmployeesByPositionAsync(string position);

    public JSONResponseDTO InsertEmployee(EmployeeDTO employeeDto);

    public JSONResponseDTO UpdateEmployee(int id,EmployeeDTO employeeDto);

    public JSONResponseDTO DeleteEmployee(int id);

    public void Save();
}
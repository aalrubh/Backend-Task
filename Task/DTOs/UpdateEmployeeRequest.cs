namespace MyApp.DTOs;

public class UpdateEmployeesRequest
{
    public required List<int> Ids { get; set; }
    public required List<EmployeeDTO> EmployeeDtos { get; set; }
}
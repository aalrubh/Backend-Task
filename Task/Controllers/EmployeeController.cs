using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyApp.Data;
using MyApp.DTOs;

namespace MyApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public EmployeeController(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    [HttpGet("id")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        try
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            return Ok(employee);
        }
        catch (Exception error)
        {
            return BadRequest(error);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var employees = await _employeeRepository.GetAllAsync(false);
            return Ok(employees);
        }
        catch (Exception error)
        {
            return BadRequest(error);
        }
    }

    [HttpPost("single")]
    public async Task<IActionResult> InsertEmployee(EmployeeDTO employeeDto)
    {
        try
        {
            if (!TryValidateModel(ModelState)) return BadRequest(ModelState);

            var result = _employeeRepository.InsertEmployee(employeeDto);
            await _employeeRepository.SaveAsync();

            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error);
        }
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> InsertEmployees(List<EmployeeDTO> employeeDtos)
    {
        try
        {
            if (!TryValidateModel(ModelState)) return BadRequest(ModelState);

            var result = _employeeRepository.InsertEmployees(employeeDtos);
            await _employeeRepository.SaveAsync();

            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error);
        }
    }

    [HttpPut("single/{id}")]
    public async Task<IActionResult> UpdateEmployee(
        [FromRoute] int id,
        [FromBody] EmployeeDTO employeeDto
    )
    {
        try
        {
            var result = _employeeRepository.UpdateEmployee(id, employeeDto);
            await _employeeRepository.SaveAsync();
            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpPut("bulk")]
    public async Task<IActionResult> UpdateEmployees([FromBody] UpdateEmployeesRequest request)
    {
        try
        {
            var result = _employeeRepository.UpdateEmployees(request.Ids, request.EmployeeDtos);
            await _employeeRepository.SaveAsync();
            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }


    [HttpDelete("single")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = _employeeRepository.DeleteEmployee(id);
            await _employeeRepository.SaveAsync();
            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpDelete("bulk")]
    public async Task<IActionResult> DeleteEmployees(List<int> ids)
    {
        try
        {
            var result = _employeeRepository.DeleteEmployees(ids);
            await _employeeRepository.SaveAsync();
            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }
}
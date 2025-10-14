using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MyApp.Data;
using MyApp.DTOs;
using Newtonsoft.Json;

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

    [HttpPost]
    public IActionResult InsertEmployee(EmployeeDTO employeeDto)
    {
        try
        {
            if (!TryValidateModel(ModelState)) return BadRequest(ModelState);

            var result =  _employeeRepository.InsertEmployee(employeeDto);
            _employeeRepository.Save();

            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error);
        }
    }

    [HttpPut]
    public IActionResult UpdateEmployee(int id, EmployeeDTO employeeDto)
    {
        try
        {
            var result =  _employeeRepository.UpdateEmployee(id, employeeDto);
            _employeeRepository.Save();
            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }

    [HttpDelete]
    public IActionResult DeleteEmployee(int id)
    {
        try
        {
            var result =  _employeeRepository.DeleteEmployee(id);
            _employeeRepository.Save();
            return Ok(result);
        }
        catch (Exception error)
        {
            return BadRequest(error.Message);
        }
    }
}
using System.Net;
using AutoMapper;
using MyApp.DTOs;
using MyApp.Models;

namespace MyApp.Data;

public class EmployeeRepository : RepositoryBase<EmployeeModel>, IEmployeeRepository
{
    private readonly IMapper _mapper;

    public EmployeeRepository(AppDBContext appDBContext, IMapper mapper) : base(appDBContext)
    {
        _mapper = mapper;
    }

    public async Task<EmployeeDTO?> GetEmployeeByIdAsync(int id)
    {
        var response = await GetAsync(e => e.Id == id, false);
        var dto = _mapper.Map<EmployeeDTO>(response.FirstOrDefault());
        return dto;
    }

    public async Task<List<EmployeeDTO>> GetEmployeesByPositionAsync(string position)
    {
        var response = await GetAsync(e => e.Position == position, false);
        var dto = _mapper.Map<List<EmployeeDTO>>(response.FirstOrDefault());
        return dto;
    }

    public JSONResponseDTO InsertEmployee(EmployeeDTO employeeDto)
    {
        try
        {
            var model = _mapper.Map<EmployeeModel>(employeeDto);
            Create(model);

            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Inserted"
            };
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public JSONResponseDTO UpdateEmployee(int id,EmployeeDTO employeeDto)
    {
        try
        {
            var model = _mapper.Map<EmployeeModel>(employeeDto);
            model.Id = id;
            Update(model);
            

            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Updated"
            };
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public JSONResponseDTO DeleteEmployee(int id)
    {
        try
        {
            var model = Get(e => e.Id == id, false).FirstOrDefault();
            if (model == null)
            {
                return new JSONResponseDTO
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Not Found"
                };
            }
            
            Delete(model);
            
            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Deleted"
            };
        }
        catch (Exception error)
        {
            throw new ApplicationException(error.Message);
        }
    }

    public void Save()
    {
        _appDBContext.SaveChanges();
    }
}
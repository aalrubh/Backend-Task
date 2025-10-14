using AutoMapper;
using MyApp.DTOs;
using MyApp.Models;

namespace MyApp.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<EmployeeModel, EmployeeDTO>().ReverseMap();
    }
}
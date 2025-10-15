using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.DTOs;
[Table("Employee")]

public class EmployeeDTO
{
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public required string Name { get; set; }
  
    [Column(TypeName = "nvarchar(100)")]
    public string? Position { get; set; }
    
    [Column(TypeName = "decimal(18,2)")]
    public decimal? Salary { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApp.Models;

[Table("Employee")]
public class EmployeeModel
{
    [Key]
    [Required]
    [Column(TypeName = "int")]
    public int Id { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public required string Name { get; set; }

    [Column(TypeName = "nvarchar(100)")] public string? Position { get; set; }

    [Column(TypeName = "decimal(18,2)")] public decimal Salary { get; set; }
}
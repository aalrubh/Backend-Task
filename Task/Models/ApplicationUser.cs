using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MyApp.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public required string FirstName { get; set; }
    
    [Required]
    [Column(TypeName = "nvarchar(100)")]
    public required string LastName { get; set; }
}
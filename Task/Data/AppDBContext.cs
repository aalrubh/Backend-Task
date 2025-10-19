using Microsoft.EntityFrameworkCore;
using MyApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MyApp.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<EmployeeModel> Employees { get; set; }
}
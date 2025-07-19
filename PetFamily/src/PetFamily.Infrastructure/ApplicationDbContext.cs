using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PetFamily.Domain.Volunteers;

namespace PetFamily.Infrastructure;

public  class ApplicationDbContext (IConfiguration configuration): DbContext
{
    public DbSet<Volunteer> Volunteers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
    }
}

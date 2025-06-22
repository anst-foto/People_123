using Microsoft.EntityFrameworkCore;
using People.Model;

namespace People.WebApi;

public class PeopleContext : DbContext
{
    public DbSet<Person> People { get; set; }
    
    public PeopleContext(DbContextOptions<PeopleContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
}
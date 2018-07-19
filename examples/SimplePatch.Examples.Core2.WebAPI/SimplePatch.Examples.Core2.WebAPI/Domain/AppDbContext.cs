using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace SimplePatch.Examples.Core2.WebAPI.Domain
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public AppDbContext() : base() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public static async Task SeedData(AppDbContext context)
        {
            await context.Database.EnsureCreatedAsync();
            if (!(await context.People.AnyAsync()))
            {
                context.People.Add(new Person() { Name = "John", Surname = "Doe", Age = 30 });
                context.People.Add(new Person() { Name = "Alyce", Surname = "Adams", Age = 25 });
                await context.SaveChangesAsync();
            }
        }
    }
}

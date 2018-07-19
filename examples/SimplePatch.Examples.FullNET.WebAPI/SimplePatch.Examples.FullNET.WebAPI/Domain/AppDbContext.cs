using System.Data.Entity;

namespace SimplePatch.Examples.FullNET.WebAPI.Domain
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public AppDbContext() : base() { }
    }
}
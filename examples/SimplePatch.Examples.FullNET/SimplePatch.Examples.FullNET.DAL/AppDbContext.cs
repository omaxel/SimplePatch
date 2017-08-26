using System.Data.Entity;

namespace SimplePatch.Examples.FullNET.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
    }
}

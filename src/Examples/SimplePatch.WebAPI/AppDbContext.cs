using SimplePatch.WebAPI.Models;
using System.Data.Entity;

namespace SimplePatch.WebAPI
{
    public class AppDbContext : DbContext
    {
        public DbSet<PersonEF> People { get; set; }
    }
}
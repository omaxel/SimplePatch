using Microsoft.EntityFrameworkCore;

namespace SimplePatch.Examples.Core.DAL
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }

        public AppDbContext() : base()
        {
            Init();
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Init();
        }

        private void Init()
        {
            Database.EnsureCreated();
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SimplePatch.Examples.Core;Trusted_Connection=True;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}



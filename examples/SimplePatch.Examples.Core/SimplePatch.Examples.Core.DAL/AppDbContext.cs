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

            if (People.CountAsync().Result == 0)
            {
                People.AddRange(
                  new Person() { Name = "Name 1", Surname = "Surname 1", Age = 21 },
                  new Person() { Name = "Name 2", Surname = "Surname 2", Age = 22 },
                  new Person() { Name = "Name 3", Surname = "Surname 3", Age = 23 },
                  new Person() { Name = "Name 4", Surname = "Surname 4", Age = 24 },
                  new Person() { Name = "Name 5", Surname = "Surname 5", Age = 25 },
                  new Person() { Name = "Name 6", Surname = "Surname 6", Age = 26 },
                  new Person() { Name = "Name 7", Surname = "Surname 7", Age = 27 },
                  new Person() { Name = "Name 8", Surname = "Surname 8", Age = 28 },
                  new Person() { Name = "Name 9", Surname = "Surname 9", Age = 29 }
                );
                SaveChanges();
            }
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=SimplePatch.Examples.Core;Trusted_Connection=True;");

            base.OnConfiguring(optionsBuilder);
        }
    }
}



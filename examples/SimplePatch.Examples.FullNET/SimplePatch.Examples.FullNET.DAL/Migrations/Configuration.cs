namespace SimplePatch.Examples.FullNET.DAL.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppDbContext context)
        {
            context.People.AddOrUpdate(
              new Person() { Id = 1, Name = "Name 1", Surname = "Surname 1", Age = 21 },
              new Person() { Id = 2, Name = "Name 2", Surname = "Surname 2", Age = 22 },
              new Person() { Id = 3, Name = "Name 3", Surname = "Surname 3", Age = 23 },
              new Person() { Id = 4, Name = "Name 4", Surname = "Surname 4", Age = 24 },
              new Person() { Id = 5, Name = "Name 5", Surname = "Surname 5", Age = 25 },
              new Person() { Id = 6, Name = "Name 6", Surname = "Surname 6", Age = 26 },
              new Person() { Id = 7, Name = "Name 7", Surname = "Surname 7", Age = 27 },
              new Person() { Id = 8, Name = "Name 8", Surname = "Surname 8", Age = 28 },
              new Person() { Id = 9, Name = "Name 9", Surname = "Surname 9", Age = 29 }
          );
        }
    }
}

namespace SimplePatch.Examples.FullNET.WebAPI.Migrations
{
    using SimplePatch.Examples.FullNET.WebAPI.Domain;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            if (!(context.People.Any()))
            {
                context.People.Add(new Person() { Name = "John", Surname = "Doe", Age = 30 });
                context.People.Add(new Person() { Name = "Alyce", Surname = "Adams", Age = 25 });
                context.SaveChanges();
            }
        }
    }
}

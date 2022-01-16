using System;
using SimplePatch.Attributes;

namespace SimplePatch.Tests
{
    internal class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        [Exclude]
        public int AgeExcludeByAttibute { get; set; }
        public int AgeExcludeByMapping { get; set; }
        public double Height { get; set; }
        public Guid Guid { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public Cool? Coolness { get; set; }
    }

    internal enum Gender
    {
        Unknown, Male, Female
    }

    internal enum Cool
    {
        Awesome, NotReally
    }

    internal class PersonConfiguration : Mapping.IEntityTypeConfiguration<Person>
    {
        public void Configuration(DeltaConfig.EntityConfig<Person> entityConfig)
        {
            entityConfig.Property(x => x.AgeExcludeByMapping).Exclude();
        }
    }

}

using SimplePatch.WebAPI.Models;
using System.Collections.Generic;

namespace SimplePatch.WebAPI
{
    public static class TestData
    {
        public static List<Person> People = new List<Person>() {
            new Person(1, "Name 1", "Surname 1", 20),
            new Person(2, "Name 2", "Surname 1", 21),
            new Person(3, "Name 3", "Surname 1", 22),
            new Person(4, "Name 4", "Surname 1", 23),
            new Person(5, "Name 5", "Surname 1", 24),
            new Person(6, "Name 6", "Surname 1", 25),
            new Person(7, "Name 7", "Surname 1", 26),
            new Person(8, "Name 8", "Surname 1", 27),
            new Person(9, "Name 9", "Surname 1", 28),
        };
    }
}
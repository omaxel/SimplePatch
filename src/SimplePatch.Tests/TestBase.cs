using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SimplePatch.Tests
{
    [TestClass]
    public class TestBase
    {
        internal Person John;

        [TestInitialize]
        public void TestInit()
        {
            John = new Person()
            {
                Name = "John",
                Surname = "Doe",
                Age = 22,
                Height = 1.7,
                BirthDate = new DateTime(1990, 2, 1, 20, 15, 10)
            };
        }
    }
}

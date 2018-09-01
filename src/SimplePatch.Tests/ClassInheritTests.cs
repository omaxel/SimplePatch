using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SimplePatch.Tests
{
    [TestClass, TestCategory(TestCategories.ClassInheritance)]
    public class ClassInheritTests : TestBase
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            DeltaConfig.Init(cfg => cfg.AddEntity<PersonExtended>());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DeltaConfig.Clean();
        }

        private class PersonExtended : Person
        {
            public string Address { get; set; }
        }

        [TestMethod]
        public void TestMethod()
        {
            const string name = "John";
            const string surname = "Doe";
            const string address = "Person address";
            
            var johnExtended = new PersonExtended();

            var delta = new Delta<PersonExtended>();
            delta.Add(x => x.Name, name);
            delta.Add(x => x.Surname, surname);
            delta.Add(x => x.Address, address);
            delta.Patch(johnExtended);

            Assert.AreEqual(name, johnExtended.Name);
            Assert.AreEqual(surname, johnExtended.Surname);
            Assert.AreEqual(address, johnExtended.Address);
        }
    }
}

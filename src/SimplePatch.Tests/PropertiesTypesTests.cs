using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static SimplePatch.Tests.DeltaUtils;

namespace SimplePatch.Tests
{
    [TestClass, TestCategory(TestCategories.PropertiesTypes)]
    public class PropertiesTypesTests : TestBase
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            DeltaConfig.Init(cfg => cfg.AddEntity<Person>());
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            DeltaConfig.Clean();
        }

        [TestMethod]
        public void IntProp()
        {
            GetDelta(x => x.Age, 23).Patch(John);
            Assert.AreEqual(23, John.Age);
        }

        [TestMethod]
        public void StringProp()
        {
            GetDelta(x => x.Name, "John Marco").Patch(John);
            Assert.AreEqual("John Marco", John.Name);
        }

        [TestMethod]
        public void DoubleProp()
        {
            GetDelta(x => x.Height, 1.65).Patch(John);
            Assert.AreEqual(1.65, John.Height);
        }

        [TestMethod]
        public void DateTimeProp()
        {
            var date = DateTime.UtcNow;
            GetDelta(x => x.BirthDate, date).Patch(John);
            Assert.AreEqual(date, John.BirthDate);
        }

        [TestMethod]
        public void GuidProp()
        {
            var guid = Guid.NewGuid();
            GetDelta(x => x.Guid, guid).Patch(John);
            Assert.AreEqual(guid, John.Guid);
        }



        #region From string

        [TestMethod]
        public void IntPropFromString()
        {
            GetDelta(x => x.Age, "28").Patch(John);
            Assert.AreEqual(28, John.Age);
        }

        [TestMethod]
        public void DoublePropFromString()
        {
            GetDelta(x => x.Height, "28,5").Patch(John);
            Assert.AreEqual(28.5, John.Height);
        }

        [TestMethod]
        public void DateTimePropFromString()
        {
            var date = DateTime.UtcNow;
            GetDelta(x => x.BirthDate, date.ToString("s")).Patch(John);
            Assert.AreEqual(date.ToString("s"), John.BirthDate.ToString("s"));
        }

        [TestMethod]
        public void GuidPropFromString()
        {
            var guid = Guid.NewGuid();
            GetDelta(x => x.Guid, guid.ToString()).Patch(John);
            Assert.AreEqual(guid, John.Guid);
        }

        #endregion
    }
}

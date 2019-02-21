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
            CreateDelta<Person, int>(x => x.Age, 23).Patch(John);
            Assert.AreEqual(23, John.Age);
        }

        [TestMethod]
        public void StringProp()
        {
            CreateDelta<Person, string>(x => x.Name, "John Marco").Patch(John);
            Assert.AreEqual("John Marco", John.Name);
        }

        [TestMethod]
        public void DoubleProp()
        {
            CreateDelta<Person, double>(x => x.Height, 1.65).Patch(John);
            Assert.AreEqual(1.65, John.Height);
        }

        [TestMethod]
        public void DateTimeProp()
        {
            var date = DateTime.UtcNow;
            CreateDelta<Person, DateTime>(x => x.BirthDate, date).Patch(John);
            Assert.AreEqual(date, John.BirthDate);
        }

        [TestMethod]
        public void GuidProp()
        {
            var guid = Guid.NewGuid();
            CreateDelta<Person, Guid>(x => x.Guid, guid).Patch(John);
            Assert.AreEqual(guid, John.Guid);
        }

        [TestMethod]
        public void EnumProp()
        {
            var gender = Gender.Male;
            CreateDelta<Person, Gender>(x => x.Gender, gender).Patch(John);
            Assert.AreEqual(gender, John.Gender);
        }

        [TestMethod]
        public void EnumNullableProp()
        {
            John.Coolness = Cool.Awesome;
            CreateDelta<Person, Cool?>(x => x.Coolness, null).Patch(John);
            Assert.IsNull(John.Coolness);
        }


        #region From string

        [TestMethod]
        public void IntPropFromString()
        {
            CreateDelta<Person, int>(x => x.Age, "28").Patch(John);
            Assert.AreEqual(28, John.Age);
        }

        [TestMethod]
        public void DoublePropFromString()
        {
            CreateDelta<Person, double>(x => x.Height, (28.5).ToString()).Patch(John);
            Assert.AreEqual(28.5, John.Height);
        }

        [TestMethod]
        public void DateTimePropFromString()
        {
            var date = DateTime.UtcNow;
            CreateDelta<Person, DateTime>(x => x.BirthDate, date.ToString("s")).Patch(John);
            Assert.AreEqual(date.ToString("s"), John.BirthDate.ToString("s"));
        }

        [TestMethod]
        public void GuidPropFromString()
        {
            var guid = Guid.NewGuid();
            CreateDelta<Person, Guid>(x => x.Guid, guid.ToString()).Patch(John);
            Assert.AreEqual(guid, John.Guid);
        }

        [TestMethod]
        public void EnumPropFromString()
        {
            var gender = Gender.Male;
            CreateDelta<Person, Gender>(x => x.Gender, gender.ToString()).Patch(John);
            Assert.AreEqual(gender, John.Gender);
        }

        [TestMethod]
        public void EnumNullablePropFromString()
        {
            John.Coolness = Cool.Awesome;
            CreateDelta<Person, Cool?>(x => x.Coolness, null).Patch(John);
            Assert.IsNull(John.Coolness);
        }

        #endregion
    }
}

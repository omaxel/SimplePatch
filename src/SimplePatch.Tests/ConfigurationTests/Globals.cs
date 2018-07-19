using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplePatch.Mapping;
using static SimplePatch.Tests.DeltaUtils;

namespace SimplePatch.Tests.ConfigurationTests
{
    [TestClass, TestCategory(TestCategories.Configuration)]
    public class Globals : TestBase
    {
        [TestCleanup]
        public void TestCleanup()
        {
            DeltaConfig.Clean();
        }

        [TestMethod]
        public void IgnoreLetterCase()
        {
            DeltaConfig.Init(cfg =>
            {
                cfg
                .IgnoreLetterCase()
                .AddEntity<Person>();
            });

            GetDelta("AgE", 23).Patch(John);
            Assert.AreEqual(23, John.Age);
        }

        [TestMethod]
        public void MappingFunction()
        {
            DeltaConfig.Init(cfg =>
            {
                cfg

                /* When the target property type is int and the input is string,
                   then the assigned value will be the length of the input string*/
                .AddMapping((propType, newValue) =>
                {
                    var result = new MapResult<object>();

                    if (propType != typeof(int)) return result.SkipMap();
                    if (newValue.GetType() != typeof(string)) return result.SkipMap();

                    result.Value = newValue.ToString().Length;

                    return result;
                })

                /* When the target property is double and the input is string,
                   then the assigned value will be the length of the string + 0.5*/
                .AddMapping((propType, newValue) =>
                {
                    var result = new MapResult<object>();

                    if (propType != typeof(double)) return result.SkipMap();

                    if (newValue.GetType() != typeof(string)) return result.SkipMap();

                    result.Value = newValue.ToString().Length + 0.5;

                    return result;
                })
                .AddEntity<Person>();
            });

            // First mapping function will be executed here, Age type is int
            GetDelta(x => x.Age, "abc").Patch(John);
            Assert.AreEqual("abc".Length, John.Age);

            // Second mapping function will be executed here, Height type is double
            GetDelta(x => x.Height, "abcdef").Patch(John);
            Assert.AreEqual("abcdef".Length + 0.5, John.Height);
        }
    }
}

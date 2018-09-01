using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimplePatch.Mapping;
using System;
using System.Linq;
using static SimplePatch.Tests.DeltaUtils;

namespace SimplePatch.Tests.ConfigurationTests
{
    [TestClass, TestCategory(TestCategories.Configuration)]
    public class Properties : TestBase
    {
        [TestCleanup]
        public void TestCleanup()
        {
            DeltaConfig.Clean();
        }

        [TestMethod]
        public void Exclude()
        {
            DeltaConfig.Init(cfg =>
            {
                cfg
                .AddEntity<Person>()
                .Property(x => x.Age).Exclude();
            });

            var initialAge = John.Age;

            CreateDelta<Person, int>(x => x.Age, 23).Patch(John);

            Assert.AreEqual(initialAge, John.Age);
        }

        [TestMethod]
        public void IgnoreNullValue()
        {
            DeltaConfig.Init(cfg =>
            {
                cfg
                .AddEntity<Person>()
                .Property(x => x.Name).IgnoreNull();
            });

            var initialName = John.Name;

            CreateDelta<Person, string>(x => x.Name, null).Patch(John);

            Assert.AreEqual(initialName, John.Name);
        }

        [TestMethod]
        public void MappingFunction()
        {
            DeltaConfig.Init(cfg =>
            {
                cfg

                /* When the target property type is string and the input is string, then the assigned value will be the reversed input string */
                .AddMapping((propType, newValue) =>
                {
                    var result = new MapResult<object>();

                    if (propType == typeof(string) && newValue.GetType() == typeof(string))
                    {
                        result.Value = string.Join("", newValue.ToString().ToCharArray().Reverse());
                    }
                    else
                    {
                        result.Skip = true;
                    }

                    return result;
                })
                .AddEntity<Person>()
                    .Property(x => x.Name)
                        /* If the input value is string, then the assigned value will be the same string. Overriding global mapping function.*/
                        .AddMapping((propType, newValue) =>
                        {
                            if (newValue.GetType() != typeof(string)) return new MapResult<string>().SkipMap();
                            return new MapResult<string>() { Value = newValue.ToString() };
                        })
                        /* If the input value is int, then the assigned value will be "number:{number}" */
                        .AddMapping((propType, newValue) =>
                        {
                            var result = new MapResult<string>();

                            if (newValue.GetType() != typeof(int)) return result.SkipMap();

                            result.Value = "number:" + newValue.ToString();

                            return result;
                        })

                        /* If the input value is DateTime, then the assigned value will be "datetime:{datetime}".
                         * This behavior could be accomplished using only the previous mapping function. They are separeted
                         * functions to test mapping functions order execution*/
                        .AddMapping((propType, newValue) =>
                        {
                            var result = new MapResult<string>();

                            if (newValue.GetType() != typeof(DateTime)) return result.SkipMap();

                            result.Value = "datetime:" + ((DateTime)newValue).ToString("s");

                            return result;
                        });
            });

            // Global mapping function executed here
            CreateDelta<Person, string>(x => x.Surname, "Rossi").Patch(John);
            Assert.AreEqual("issoR", John.Surname);

            // First property mapping function executed here
            CreateDelta<Person, string>(x => x.Name, "Mario").Patch(John);
            Assert.AreEqual("Mario", John.Name);

            // Second property mapping function executed here
            CreateDelta<Person, string>(x => x.Name, 15).Patch(John);
            Assert.AreEqual("number:15", John.Name);

            // Third property mapping function executed here
            CreateDelta<Person, string>(x => x.Name, John.BirthDate).Patch(John);
            Assert.AreEqual("datetime:1990-02-01T20:15:10", John.Name);
        }
    }
}

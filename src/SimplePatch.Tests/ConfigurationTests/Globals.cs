using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SimplePatch.Mapping;
using static SimplePatch.Tests.DeltaUtils;

namespace SimplePatch.Tests.ConfigurationTests
{
    [TestClass, TestCategory(TestCategories.Configuration)]
    public class Globals : TestBase
    {
        internal class Data
        {
            public string Category { get; set; }

            public Person PersonInfo { get; set; }
        }

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

            CreateDelta<Person>("AgE", 23).Patch(John);
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
            CreateDelta<Person, int>(x => x.Age, "abc").Patch(John);
            Assert.AreEqual("abc".Length, John.Age);

            // Second mapping function will be executed here, Height type is double
            CreateDelta<Person, double>(x => x.Height, "abcdef").Patch(John);
            Assert.AreEqual("abcdef".Length + 0.5, John.Height);
        }

        [TestMethod]
        public void MappingToDeltaType()
        {
            DeltaConfig.Init(cfg =>
            {
                cfg.AddEntity<Person>();
                cfg.AddEntity<Data>();
                cfg.AddMapping((propType, newValue) =>
                {
                    var result = new MapResult<object>();
                    if (!(newValue is JToken jToken)) return result.SkipMap();

                    var deltaOfEntity = (typeof(Delta<>)).MakeGenericType(propType);
                    result.Value = jToken.ToObject(deltaOfEntity);
                    return result;
                });
            });

            var data = new Data
            {
                Category = "Test",
                PersonInfo = John
            };

            var dataDelta = new Delta<Data>
            {
                [nameof(Data.PersonInfo)] = JObject.FromObject(new Dictionary<string, object>{[nameof(Person.Name)] = "Foo"})
            };

            dataDelta.Patch(data);
            Assert.AreEqual("Foo", data.PersonInfo.Name);
            Assert.AreEqual("Doe", data.PersonInfo.Surname);
        }
    }
}

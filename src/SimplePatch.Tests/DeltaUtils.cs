using System;
using System.Linq.Expressions;

namespace SimplePatch.Tests
{
    internal class DeltaUtils
    {
        internal static Delta<Person> GetDelta<TProp, T>(Expression<Func<Person, TProp>> property, T propValue)
        {
            var delta = new Delta<Person>();
            delta.Add(property, propValue);
            return delta;
        }

        internal static Delta<Person> GetDelta<T>(string propertyName, T propValue)
        {
            var delta = new Delta<Person>();
            delta.Add(propertyName, propValue);
            return delta;
        }

    }
}

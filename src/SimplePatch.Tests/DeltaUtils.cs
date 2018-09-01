using System;
using System.Linq.Expressions;

namespace SimplePatch.Tests
{
    internal class DeltaUtils
    {
        internal static Delta<TEntity> CreateDelta<TEntity, TProp>(Expression<Func<TEntity, TProp>> property, object propValue)
            where TEntity : class, new()
        {
            var delta = new Delta<TEntity>();
            delta.Add(property, propValue);
            return delta;
        }

        internal static Delta<TEntity> CreateDelta<TEntity>(string property, object propValue)
          where TEntity : class, new()
        {
            var delta = new Delta<TEntity>();
            delta.Add(property, propValue);
            return delta;
        }
    }
}

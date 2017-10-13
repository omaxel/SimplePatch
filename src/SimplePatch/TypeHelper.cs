using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePatch
{
    internal class TypeHelper
    {
        public static IEnumerable<DeltaInfo> GetEntityProperties<TEntity>()
        {
            return typeof(TEntity).GetTypeInfo().DeclaredProperties.Where(x => x.GetMethod.IsPublic && x.SetMethod.IsPublic && x.CanRead && x.CanWrite).Select(x => new DeltaInfo(x));
        }

        /// <summary>
        /// Returns the type specified by the parameter or type below if <paramref name="type" /> is <see cref="Nullable" />.
        /// </summary>
        /// <param name="type">The type to be verified.</param>
        /// <returns>The type specified by the parameter or type below if <paramref name="type" /> is <see cref="Nullable" />.</returns>
        public static Type GetTrueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Indicates whether the specified type accepts null values.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the specified type accepts null values, otherwise false.</returns>
        public static bool IsNullable(Type type)
        {
            if (!type.GetTypeInfo().IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false;
        }
    }
}

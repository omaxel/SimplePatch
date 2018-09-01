using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePatch.Helpers
{
    internal class TypeHelper
    {
        /// <summary>
        /// Obtains the list of properties belonging to the specified identity as <see cref="IEnumerable{DeltaInfo}"/>
        /// </summary>
        /// <typeparam name="TEntity">Entity for which obtain the properties</typeparam>
        /// <returns>List of properties belonging to the specified identity as <see cref="IEnumerable{DeltaInfo}"/></returns>
        internal static IEnumerable<DeltaPropInfo> GetEntityProperties<TEntity>()
        {
            return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetMethod != null && x.GetMethod.IsPublic && x.SetMethod != null && x.SetMethod.IsPublic && x.CanRead && x.CanWrite).Select(x => new DeltaPropInfo(x));
        }


        /// <summary>
        /// Returns the type specified by the parameter or type below if <paramref name="type" /> is <see cref="Nullable" />.
        /// </summary>
        /// <param name="type">The type to be verified.</param>
        /// <returns>The type specified by the parameter or type below if <paramref name="type" /> is <see cref="Nullable" />.</returns>
        internal static Type GetTrueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        /// <summary>
        /// Indicates whether the specified type accepts null values.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the specified type accepts null values, otherwise false.</returns>
        internal static bool IsNullable(Type type)
        {
            if (!type.GetTypeInfo().IsValueType) return true; // ref-type
            if (Nullable.GetUnderlyingType(type) != null) return true; // Nullable<T>
            return false;
        }
    }
}

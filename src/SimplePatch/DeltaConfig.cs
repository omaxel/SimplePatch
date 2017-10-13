using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace SimplePatch
{
    public class DeltaConfig
    {
        internal static bool IgnoreLetterCase = false;

        public static void Init(Action<Config> config)
        {
            config(new Config());
        }

        public sealed class Config
        {
            /// <summary>
            /// Sets the properties to exclude when calling <see cref="Delta{TEntity}.Patch(TEntity)"/>.
            /// </summary>
            /// <typeparam name="T">Class in which the property is contained.</typeparam>
            /// <param name="properties">Properties to exclude when calling <see cref="Delta{TEntity}.Patch(TEntity)"/></param>
            /// <returns></returns>
            public Config ExcludeProperties<T>(params Expression<Func<T, object>>[] properties)
            {
                var type = typeof(T);

                var propList = new List<string>();
                foreach (var item in properties)
                {
                    var propertyInfo = GetMemberExpression(item).Member as PropertyInfo;
                    propList.Add(propertyInfo.Name);
                }

                DeltaCache.excludedProperties.TryAdd(type.FullName, propList.ToArray());

                return this;
            }

            /// <summary>
            /// Specifies properties for whose null value will be ignored.
            /// </summary>
            /// <typeparam name="T">Class in which the property is contained.</typeparam>
            /// <param name="properties">Properties for whose null value will be ignored.</param>
            /// <returns></returns>
            public Config IgnoreNullValue<T>(params Expression<Func<T, object>>[] properties)
            {
                var type = typeof(T);

                var propList = TypeHelper.GetEntityProperties<T>().ToList();
                foreach (var prop in properties)
                {
                    var propertyInfo = GetMemberExpression(prop).Member as PropertyInfo;

                    propList.First(x => x.Name == propertyInfo.Name).IgnoreNullValue = true;
                }

                DeltaCache.entityProperties.TryAdd(type.FullName, propList);

                return this;
            }

            /// <summary>
            /// If enabled, the properties names comparing function will ignore letter case.
            /// </summary>
            /// <param name="enabled">Whetever to ignore letter case for properties.</param>
            /// <returns></returns>
            public Config IgnoreLetterCase(bool enabled = true)
            {
                DeltaConfig.IgnoreLetterCase = enabled;
                return this;
            }

            private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> exp)
            {
                var member = exp.Body as MemberExpression;
                var unary = exp.Body as UnaryExpression;
                return member ?? (unary != null ? unary.Operand as MemberExpression : null);
            }
        }
    }
}

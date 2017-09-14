using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace SimplePatch
{
    public class DeltaConfig
    {
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

                string typeFullname = typeof(T).FullName;

                DeltaCache.excludedProperties.TryAdd(typeFullname, propList.ToArray());

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

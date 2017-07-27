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
            public void ExcludeProperties<T>(params Expression<Func<T, object>>[] properties)
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

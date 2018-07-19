using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SimplePatch.Helpers
{
    internal class ExpressionHelper
    {
        /// <summary>
        /// Gets the name of the property specified in the expression
        /// </summary>
        /// <typeparam name="T">Type of the entity containing the property</typeparam>
        /// <typeparam name="T2">Type of the property</typeparam>
        /// <param name="exp">Expression indicating the property</param>
        /// <returns>The name of the specified property</returns>
        internal static string GetPropertyName<T , T2>(Expression<Func<T, T2>> exp)
        {
            var member = exp.Body as MemberExpression;
            var unary = exp.Body as UnaryExpression;
            return ((member ?? (unary != null ? unary.Operand as MemberExpression : null)).Member as PropertyInfo).Name;
        }
    }
}

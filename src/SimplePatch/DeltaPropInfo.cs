using SimplePatch.Mapping;
using System.Collections.Generic;
using System.Reflection;

namespace SimplePatch
{
    internal class DeltaPropInfo
    {
        internal bool IgnoreNullValue { get; set; }
        internal PropertyInfo PropertyInfo { get; set; }

        internal string Name { get => PropertyInfo.Name; }

        internal List<MapDelegate<object>> MapFunctions { get; set; }

        internal bool Excluded { get; set; }

        internal DeltaPropInfo(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }
    }
}

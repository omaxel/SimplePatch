using System.Reflection;

namespace SimplePatch
{
    internal class DeltaInfo
    {
        public bool IgnoreNullValue { get; set; } = false;
        public PropertyInfo PropertyInfo { get; set; }

        public string Name { get => PropertyInfo.Name; }

        public DeltaInfo(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }
    }
}

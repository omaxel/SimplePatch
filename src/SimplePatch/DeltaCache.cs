using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace SimplePatch
{
    internal static class DeltaCache
    {
        public static ConcurrentDictionary<string, IEnumerable<PropertyInfo>> entityProperties = new ConcurrentDictionary<string, IEnumerable<PropertyInfo>>();
        public static ConcurrentDictionary<string, string[]> excludedProperties = new ConcurrentDictionary<string, string[]>();
    }
}
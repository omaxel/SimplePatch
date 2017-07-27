using System.Collections.Concurrent;
using System.Reflection;

namespace SimplePatch
{
    internal static class DeltaCache
    {
        public static ConcurrentDictionary<string, PropertyInfo[]> entityProperties = new ConcurrentDictionary<string, PropertyInfo[]>();
        public static ConcurrentDictionary<string, string[]> excludedProperties = new ConcurrentDictionary<string, string[]>();
    }
}
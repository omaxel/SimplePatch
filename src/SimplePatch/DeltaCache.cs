using SimplePatch.Helpers;
using SimplePatch.Mapping;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimplePatch
{
    internal static class DeltaCache
    {
        private static ConcurrentDictionary<string, List<DeltaPropInfo>> EntityProperties = new ConcurrentDictionary<string, List<DeltaPropInfo>>();

        internal static string GetEntityKey<TEntity>() where TEntity : class, new()
        {
            return typeof(TEntity).FullName;
        }

        internal static List<DeltaPropInfo> GetEntityProperties<TEntity>() where TEntity : class, new()
        {
            var key = GetEntityKey<TEntity>();
            if (!EntityProperties.ContainsKey(key)) return null;
            return EntityProperties[key];
        }

        internal static void AddEntity<TEntity>() where TEntity : class, new()
        {
            // Adding entity properties to cache
            EntityProperties.TryAdd(GetEntityKey<TEntity>(), TypeHelper.GetEntityProperties<TEntity>().ToList());
        }

        internal class PropertyEditor<TEntity, TProp> where TEntity : class, new()
        {
            private readonly DeltaPropInfo deltaInfo;

            internal PropertyEditor(string propertyName)
            {
                deltaInfo = EntityProperties[GetEntityKey<TEntity>()].Find(x => x.Name == propertyName);
            }

            internal void Exclude()
            {
                deltaInfo.Excluded = true;
            }

            internal void IgnoreNullValue()
            {
                deltaInfo.IgnoreNullValue = true;
            }

            internal void AddMapping(MapDelegate<TProp> mapFunction)
            {
                if (deltaInfo.MapFunctions == null) deltaInfo.MapFunctions = new List<MapDelegate<object>>();
                deltaInfo.MapFunctions.Add((propertyType, newValue) => (MapResult<object>)mapFunction(propertyType, newValue));
            }
        }

        internal static void Clear()
        {
            EntityProperties.Clear();
        }
    }
}
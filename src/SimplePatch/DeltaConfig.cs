using SimplePatch.Helpers;
using SimplePatch.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SimplePatch
{
    public class DeltaConfig
    {
        internal static bool IgnoreLetterCase = false;

        /// <summary>
        /// List of the global mapping function
        /// </summary>
        internal static List<MapDelegate<object>> GlobalMappings;

        public static void Init(Action<Config> config)
        {
            config(new Config());
        }

        internal static void Clean()
        {
            IgnoreLetterCase = false;
            GlobalMappings?.Clear();
            DeltaCache.Clear();
        }

        public sealed class Config
        {
            public sealed class EntityConfig<T> where T : class, new()
            {
                public PropertyConfig<T, TProp> Property<TProp>(Expression<Func<T, TProp>> property)
                {
                    return new PropertyConfig<T, TProp>(property);
                }
            }

            public sealed class PropertyConfig<TEntity, TProp> where TEntity : class, new()
            {
                private readonly string propertyName;
                private readonly DeltaCache.PropertyEditor<TEntity, TProp> deltaCachePropertyEditor;

                public PropertyConfig(Expression<Func<TEntity, TProp>> property)
                {
                    propertyName = ExpressionHelper.GetPropertyName(property);
                    deltaCachePropertyEditor = new DeltaCache.PropertyEditor<TEntity, TProp>(propertyName);
                }

                /// <summary>
                /// Adds a mapping function for the specified property of the specified entity.
                /// </summary>
                /// <typeparam name="TProp">Type of the property</typeparam>
                /// <param name="property">Expression which indicates the property</param>
                /// <param name="mapFunction">Mapping function used to evaluate the value to be assigned to the property</param>
                /// <returns></returns>
                public PropertyConfig<TEntity, TProp> AddMapping(MapDelegate<TProp> mapFunction)
                {
                    deltaCachePropertyEditor.AddMapping(mapFunction);
                    return this;
                }

                /// <summary>
                /// Ignore null value for the specified property
                /// </summary>
                /// <returns></returns>
                public PropertyConfig<TEntity, TProp> IgnoreNull()
                {
                    deltaCachePropertyEditor.IgnoreNullValue();
                    return this;
                }

                /// <summary>
                /// Marks the specified property as excluded when calling <see cref="Delta{TEntity}.Patch(TEntity)"/>.
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="properties"></param>
                /// <returns></returns>
                public PropertyConfig<TEntity, TProp> Exclude()
                {
                    deltaCachePropertyEditor.Exclude();
                    return this;
                }
            }

            /// <summary>
            /// Allows to add settings for the specified property
            /// </summary>
            /// <typeparam name="T">Type of the property for which add settings</typeparam>
            /// <returns></returns>
            public EntityConfig<T> AddEntity<T>() where T : class, new()
            {
                DeltaCache.AddEntity<T>();
                return new EntityConfig<T>();
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

            /// <summary>
            /// Adds a global mapping function which will be executed for every property of the processed entities.
            /// The result of the <paramref name="mappingFunc"/> will be used to evaluate the value to be assigned to the processed property.
            /// </summary>
            /// <param name="mappingFunc">The mapping function</param>
            /// <returns></returns>
            public Config AddMapping(MapDelegate<object> mappingFunc)
            {
                if (GlobalMappings == null) GlobalMappings = new List<MapDelegate<object>>();
                GlobalMappings.Add(mappingFunc);
                return this;
            }
        }
    }
}

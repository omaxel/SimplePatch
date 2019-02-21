using SimplePatch.Helpers;
using SimplePatch.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SimplePatch
{
    public sealed class Delta<TEntity> : IDictionary<string, object> where TEntity : class, new()
    {
        /// <summary>
        /// Contains the property-value pair for the <see cref="TEntity"/> entity.
        /// </summary>
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        private List<DeltaPropInfo> entityProperties;

        public object this[string key]
        {
            get => dict[key];
            set
            {
                if (DeltaConfig.IgnoreLetterCase)
                {
                    //Get the correct case property name
                    var propertyName = GetCorrectCasePropertyName(key);

                    //Set the value if the property name is found in the entity (ignoring letter case)
                    if (propertyName != null) dict[propertyName] = value;
                }
                else
                {
                    if (IsPropertyAllowed(key)) dict[key] = value;
                }
            }
        }

        public Delta() : base()
        {
            entityProperties = DeltaCache.GetEntityProperties<TEntity>();

            if (entityProperties == null) throw new Exception($"Entity {typeof(TEntity).Name} ({typeof(TEntity).FullName}) must be declared in DeltaConfig.Init(cfg => cfg.AddEntity<T>()) method.");
        }

        /// <summary>
        /// Create a new <see cref="TEntity" /> instance and fill the properties for which you have a value.
        /// </summary>
        /// <returns>The <see cref="TEntity"/> instance</returns>
        public TEntity GetEntity()
        {
            return SetPropertiesValue(new TEntity());
        }

        /// <summary>
        /// Sets the value of the modified properties for the specified entity.
        /// </summary>
        /// <param name="entity">Entity to be edited.</param>
        public void Patch(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException();
            entity = SetPropertiesValue(entity);
        }

        /// <summary>
        /// Indicates whether the specified property name is present in the list of changed property names.
        /// </summary>
        /// <param name="propertyName">The name of the property to be verified.</param>
        /// <returns>True if the property name is present in the list of changed property names, otherwise False.</returns>
        public bool HasProperty(string propertyName)
        {
            return dict.ContainsKey(propertyName);
        }

        /// <summary>
        /// Try to get the property value with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property to get the value.</param>
        /// <param name="propertyValue">The value of the specified property.</param>
        /// <returns>True if it was possible to get the property value, otherwise False.</returns>
        public bool TryGetPropertyValue(string propertyName, out object propertyValue)
        {
            if (!HasProperty(propertyName))
            {
                propertyValue = null;
                return false;
            }

            propertyValue = dict[propertyName];
            return true;
        }

        /// <summary>
        /// Adds an element to the dictionary only if the specified key is a property name of <see cref="TEntity"/>.
        /// </summary>
        /// <param name="item">Item to be added. The element will not be added if <paramref name="item"/>.Value is null or it is equal to <see cref="string.Empty"/>. See <see cref="IsPropertyAllowed(string)".</param>
        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary only if the specified key is a property name of <see cref="TEntity"/>.
        /// </summary>
        /// <param name="key">Element key to add.</param>
        /// <param name="value">Value of element to be added. The element will not be added if null or equal to <see cref="string.Empty"/>. See <see cref="IsPropertyAllowed(string)".</param>
        public void Add(string key, object value)
        {
            if (DeltaConfig.IgnoreLetterCase)
            {
                //Get the correct case property name
                var propertyName = GetCorrectCasePropertyName(key);

                //Set the value if the property name is found in the entity (ignoring letter case)
                if (propertyName != null) dict[propertyName] = value;
            }
            else
            {
                if (IsPropertyAllowed(key)) dict.Add(key, value);
            }
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary only if the specified key is a property name of <see cref="TEntity"/>.
        /// </summary>
        /// <param name="property">Element key to add.</param>
        /// <param name="value">Value of element to be added. The element will not be added if null or equal to <see cref="string.Empty"/>. See <see cref="IsPropertyAllowed(string)".</param>
        public void Add<TProp>(Expression<Func<TEntity, TProp>> property, object value)
        {
            var propertyName = ExpressionHelper.GetPropertyName(property);
            Add(propertyName, value);
        }

        /// <summary>
        /// Returns the properties that have been specified (compared to <see cref="TEntity"/> properties) as an enumeration of property names.
        /// </summary>
        /// <returns>The property names.</returns>
        public IEnumerable<string> GetSpecifiedPropertyNames()
        {
            foreach (var item in dict)
            {
                yield return item.Key;
            }
        }

        /// <summary>
        /// Returns the properties that haven't been specified (compared to <see cref="TEntity"/> properties) as an enumeration of property names.
        /// </summary>
        /// <returns>The property names.</returns>
        public IEnumerable<string> GetNotSpecifiedPropertyNames()
        {
            return entityProperties.Select(x => x.Name).Where(x => !dict.ContainsKey(x));
        }

        #region Private methods

        /// <summary>
        /// Indicates whether <see cref="TEntity" /> exposes a property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be verified.</param>
        /// <returns>True if <see cref="TEntity" /> exposes a property with the specified name, otherwise False.</returns>
        private bool IsPropertyAllowed(string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && entityProperties.Any(x => x.Name == propertyName);
        }

        /// <summary>
        /// Return the property name with correct case starting from an incorrect case name.
        /// </summary>
        /// <param name="propertyName">The property name </param>
        /// <returns>The correct case property name. If no property found, null.</returns>
        private string GetCorrectCasePropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)) return null;

            var propertyNameUpperCase = propertyName.ToUpper();
            foreach (var property in entityProperties)
            {
                if (string.Equals(property.Name, propertyNameUpperCase, StringComparison.OrdinalIgnoreCase)) return property.Name;
            }

            return null;
        }

        /// <summary>
        /// Set the value for each property of <see cref="TEntity" /> for which there is a reference in <see cref="dict" />.
        /// </summary>
        /// <param name="entity">The instance of <see cref="TEntity" /> to set properties.</param>
        /// <returns>The modified entity.</returns>
        private TEntity SetPropertiesValue(TEntity entity)
        {
            foreach (var prop in entityProperties)
            {
                var propertyInfo = prop.PropertyInfo;
                if (ContainsKey(propertyInfo.Name) && !prop.Excluded)
                {
                    var truePropertyType = TypeHelper.GetTrueType(propertyInfo.PropertyType);
                    var newPropertyValue = this[propertyInfo.Name];

                    //Check for null value before getting type of new value
                    if (newPropertyValue == null)
                    {
                        if (prop.IgnoreNullValue) continue;

                        //Check if destination property allows null value
                        if (TypeHelper.IsNullable(propertyInfo.PropertyType))
                        {
                            var valueFromMappingsForNull = GetValueFromAllMappings(typeof(TEntity), prop, newPropertyValue);

                            if (!valueFromMappingsForNull.Skip)
                            {
                                propertyInfo.SetValue(entity, valueFromMappingsForNull.Value);
                            }
                            else
                            {
                                propertyInfo.SetValue(entity, null);
                            }
                            continue;
                        }
                        else
                        {
                            throw new Exception($"Null value not allowed for '{propertyInfo.Name}' property  of '{typeof(TEntity).FullName}'");
                        }
                    }

                    var valueFromMappings = GetValueFromAllMappings(typeof(TEntity), prop, newPropertyValue);

                    if (!valueFromMappings.Skip)
                    {
                        propertyInfo.SetValue(entity, valueFromMappings.Value);
                    }
                    // If no mapping function assigned a value to the property, use the default mapping
                    else
                    {
                        var newPropertyValueType = newPropertyValue.GetType();

                        // Guid from string
                        if (truePropertyType == typeof(Guid) && newPropertyValueType == typeof(string))
                        {
                            newPropertyValue = new Guid((string)newPropertyValue);
                            propertyInfo.SetValue(entity, newPropertyValue);
                        }
                        // Enum from string
                        else if (truePropertyType.GetTypeInfo().IsEnum && newPropertyValueType == typeof(string))
                        {
                            newPropertyValue = Enum.Parse(truePropertyType, (string)newPropertyValue);
                            propertyInfo.SetValue(entity, newPropertyValue);
                        }
                        else
                        {
                            propertyInfo.SetValue(entity, Convert.ChangeType(newPropertyValue, truePropertyType));
                        }
                    }
                }
            }

            return entity;
        }

        /// <summary>
        /// Obtain the <see cref="MapResult{T}"/> from <see cref="GetValueFromPropertyMappings(Type, Type, string, object)"/> or <see cref="GetValueFromGlobalMappings(Type, object)"/>.
        /// if <see cref="GetValueFromPropertyMappings(Type, Type, string, object)"/> returns a <see cref="MapResult{T}"/> with <see cref="MapResult{T}.Skip"/> = false, then it will be return.
        /// Otherwise the result of <see cref="GetValueFromGlobalMappings(Type, object)"/> will be returned.
        /// </summary>
        /// <param name="entityType">Type of the entity</param>
        /// <param name="deltaPropInfo">Informations about the property</param>
        /// <param name="newPropertyValue">New value which should be processed before assigning it to the processed property</param>
        /// <returns></returns>
        private MapResult<object> GetValueFromAllMappings(Type entityType, DeltaPropInfo deltaPropInfo, object newPropertyValue)
        {
            var valueFromPropertyMappings = GetValueFromPropertyMappings(deltaPropInfo, newPropertyValue);
            if (!valueFromPropertyMappings.Skip) return valueFromPropertyMappings;

            return GetValueFromGlobalMappings(deltaPropInfo.PropertyInfo.PropertyType, newPropertyValue);
        }

        /// <summary>
        /// Obtains the first <see cref="MapResult{T}"/> from global mapping functions which handles the specified <paramref name="propertyType"/> and <paramref name="newPropertyValue"/>.
        /// </summary>
        /// <param name="propertyType">Type of the property to be processed</param>
        /// <param name="newPropertyValue">New value which should be processed before assigning it to the processed property</param>
        /// <returns></returns>
        private MapResult<object> GetValueFromGlobalMappings(Type propertyType, object newPropertyValue)
        {
            var mappings = DeltaConfig.GlobalMappings;

            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    var mapResult = mapping(propertyType, newPropertyValue);

                    if (mapResult.Skip) continue;

                    return mapResult;
                }
            }

            return new MapResult<object>() { Skip = true };
        }

        /// <summary>
        /// Obtains the first <see cref="MapResult{T}"/> from the mapping functions linked to the specified property.
        /// </summary>
        /// <param name="entityType">Type of the entity</param>
        /// <param name="propertyType">Type of the property</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="newPropertyValue">New value which should be processed before assigning it to the processed property</param>
        /// <returns></returns>
        private MapResult<object> GetValueFromPropertyMappings(DeltaPropInfo deltaPropInfo, object newPropertyValue)
        {
            var mappings = deltaPropInfo.MapFunctions;

            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    var mapResult = mapping(deltaPropInfo.PropertyInfo.PropertyType, newPropertyValue);

                    if (mapResult.Skip) continue;

                    return mapResult;
                }
            }

            return new MapResult<object>().SkipMap();
        }

        #endregion

        #region Implementing the IDictionary Interface
        public ICollection<string> Keys => dict.Keys;
        public ICollection<object> Values => dict.Values;
        public int Count => dict.Count;
        public bool IsReadOnly => ((IDictionary<string, object>)dict).IsReadOnly;

        public bool ContainsKey(string key)
        {
            return dict.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return dict.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return dict.TryGetValue(key, out value);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IDictionary<string, object>)dict).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return dict.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dict.GetEnumerator();
        }
        #endregion
    }
}

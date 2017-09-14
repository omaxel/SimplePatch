using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePatch
{
    public sealed class Delta<TEntity> : IDictionary<string, object> where TEntity : class, new()
    {
        /// <summary>
        /// Contains the property-value pair for the <see cref="TEntity"/> entity.
        /// </summary>
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        /// <summary>
        /// The full name of the type <see cref="TEntity"/>.
        /// </summary>
        private string typeFullName;

        public object this[string key]
        {
            get => dict[key];
            set
            {
                if (IsPropertyAllowed(key)) dict[key] = value;
            }
        }

        public Delta() : base()
        {
            typeFullName = typeof(TEntity).FullName;

            DeltaCache.entityProperties.TryAdd(typeFullName, typeof(TEntity).GetTypeInfo().DeclaredProperties.Where(x => x.GetMethod.IsPublic && x.SetMethod.IsPublic && x.CanRead && x.CanWrite));
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
            if (IsPropertyAllowed(item.Key)) dict.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary only if the specified key is a property name of <see cref="TEntity"/>.
        /// </summary>
        /// <param name="key">Element key to add.</param>
        /// <param name="value">Value of element to be added. The element will not be added if null or equal to <see cref="string.Empty"/>. See <see cref="IsPropertyAllowed(string)".</param>
        public void Add(string key, object value)
        {
            if (IsPropertyAllowed(key)) dict.Add(key, value);
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
            return DeltaCache.entityProperties[typeFullName].Select(x => x.Name).Where(x => !dict.ContainsKey(x));
        }

        #region Private methods

        /// <summary>
        /// Indicates whether <see cref="TEntity" /> exposes a property with the specified name.
        /// </summary>
        /// <param name="propertyName">The name of the property to be verified.</param>
        /// <returns>True if <see cref="TEntity" /> exposes a property with the specified name, otherwise False.</returns>
        private bool IsPropertyAllowed(string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && DeltaCache.entityProperties[typeFullName].Any(x => x.Name == propertyName);
        }

        /// <summary>
        /// Set the value for each property of <see cref="TEntity" /> for which there is a reference in <see cref="dict" />.
        /// </summary>
        /// <param name="entity">The instance of <see cref="TEntity" /> to set properties.</param>
        /// <returns>The modified entity.</returns>
        private TEntity SetPropertiesValue(TEntity entity)
        {
            //If the cache contains the property list for the specified type, set the properties value
            if (DeltaCache.entityProperties.TryGetValue(typeFullName, out var properties))
            {
                foreach (var prop in properties)
                {
                    if (ContainsKey(prop.Name) && !IsExcludedProperty(typeFullName, prop.Name))
                    {
                        var propertyType = GetTrueType(prop.PropertyType);
                        var newPropertyValue = this[prop.Name];

                        var newPropertyValueType = newPropertyValue.GetType();

                        //Guid from string
                        if (propertyType == typeof(Guid) && newPropertyValueType == typeof(string))
                        {
                            newPropertyValue = new Guid((string)newPropertyValue);
                            prop.SetValue(entity, newPropertyValue, null);
                        }
                        else
                        {
                            prop.SetValue(entity, Convert.ChangeType(newPropertyValue, propertyType), null);
                        }
                    }
                }

                return entity;
            }

            throw new Exception("Entity properties not added to cache. Problems with Delta<T> constructor?");
        }

        /// <summary>
        /// Specifies whether the change must be disabled for the property with specified name belonging to the specified entity.
        /// </summary>
        /// <param name="typeFullName">The entity's full name that exposes the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>True if property is excluded from changes, otherwise False.</returns>
        private bool IsExcludedProperty(string typeFullName, string propertyName)
        {
            if (!DeltaCache.excludedProperties.ContainsKey(typeFullName)) return false;
            if (DeltaCache.excludedProperties[typeFullName].Contains(propertyName)) return true;
            return false;
        }

        /// <summary>
        /// Returns the type specified by the parameter or type below if <paramref name="type" /> is <see cref="Nullable" />.
        /// </summary>
        /// <param name="type">The type to be verified.</param>
        /// <returns>The type specified by the parameter or type below if <paramref name="type" /> is <see cref="Nullable" />.</returns>
        private Type GetTrueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
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

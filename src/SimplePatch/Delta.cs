using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimplePatch
{
    public sealed class Delta<TEntity> : IDictionary<string, object> where TEntity : class, new()
    {
        private Dictionary<string, object> dict = new Dictionary<string, object>();

        /// <summary>
        /// Il nome completo del tipo <see cref="TEntity"/>
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

            DeltaCache.entityProperties.TryAdd(typeFullName, typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public));
        }

        /// <summary>
        /// Crea una nuova entità del tipo <see cref="TEntity"/> e popola le proprietà per cui si dispone un valore.
        /// </summary>
        /// <returns>Una nuova entità di tipo <see cref="TEntity"/></returns>
        public TEntity GetEntity()
        {
            return SetPropertiesValue(new TEntity());
        }

        /// <summary>
        /// Imposta il valore delle proprietà modificate per l'entità specificata.
        /// </summary>
        /// <param name="entity">L'entità da modificare.</param>
        public void Patch(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException();
            entity = SetPropertiesValue(entity);
        }

        /// <summary>
        /// Indica se il nome della proprietà specificata è presente nella lista dei nomi delle proprietà modificate.
        /// </summary>
        /// <param name="propertyName">Il nome della proprietà da verificare</param>
        /// <returns>True se il nome della proprietà è presente nella lista dei nomi delle proprietà modificate, altrimenti False</returns>
        public bool HasProperty(string propertyName)
        {
            return dict.ContainsKey(propertyName);
        }

        /// <summary>
        /// Tenta di ottenere il valore della proprietà con nome specificato.
        /// </summary>
        /// <param name="propertyName">Il nome della proprietà per cui ottenere il valore.</param>
        /// <param name="propertyValue">Il valore della proprietà specificata.</param>
        /// <returns>True se è stato possibile ottenere il valore della proprietà, altrimenti False.</returns>
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
        /// Aggiunge un elemento al dizionario solo se la chiave specificata è un nome di proprietà di <see cref="TEntity"/>.
        /// </summary>
        /// <param name="item">Elemento da aggiungere. Se <paramref name="item"/>.Value è null o <see cref="string.Empty"/> l'elemento non verrà aggiunto. Vedi <see cref="IsPropertyAllowed(string)".</param>
        public void Add(KeyValuePair<string, object> item)
        {
            if (IsPropertyAllowed(item.Key)) dict.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Aggiunge la chiave e il valore specificati al dizionario solo se la chiave specificata è un nome di proprietà di <see cref="TEntity"/>.
        /// </summary>
        /// <param name="key">Chiave dell'elemento da aggiungere.</param>
        /// <param name="value">Valore dell'elemento da aggiungere. Se null o <see cref="string.Empty"/> l'elemento non verrà aggiunto. Vedi <see cref="IsPropertyAllowed(string)"./></param>
        public void Add(string key, object value)
        {
            if (IsPropertyAllowed(key)) dict.Add(key, value);
        }

        /// <summary>
        /// Indica se <see cref="TEntity"/> espone una proprietà con il nome specificato.
        /// </summary>
        /// <param name="propertyName">Il nome della proprietà da verificare.</param>
        /// <returns>True se <see cref="TEntity"/> espone una proprietà con il nome specificato, altrimenti False</returns>
        private bool IsPropertyAllowed(string propertyName)
        {
            return !string.IsNullOrEmpty(propertyName) && DeltaCache.entityProperties[typeFullName].Any(x => x.Name == propertyName);
        }

        /// <summary>
        /// Imposta il valore per ogni proprietà di <see cref="TEntity"/> per cui esiste un riferimento in <see cref="dict"/>.
        /// </summary>
        /// <param name="entity">L'istanza di <see cref="TEntity"/> per cui impostare le proprietà.</param>
        /// <returns>L'entità modificata</returns>
        private TEntity SetPropertiesValue(TEntity entity)
        {
            //Se la cache non contiene la lista delle proprietà per il tipo specificato, aggiungo le proprietà
            if (DeltaCache.entityProperties.TryGetValue(typeFullName, out var properties))
            {
                foreach (var prop in properties)
                {
                    if (ContainsKey(prop.Name) && !IsExcludedProperty(typeFullName, prop.Name))
                    {
                        var propertyType = GetTrueType(prop.PropertyType);
                        var newPropertyvalue = this[prop.Name];

                        prop.SetValue(entity, Convert.ChangeType(newPropertyvalue, propertyType), null);
                    }
                }

                return entity;
            }

            throw new Exception("Entity properties not added to cache. Problems with Delta<T> constructor?");
        }

        /// <summary>
        /// Indica se, per la proprietà con nome specificato appartenente all'entità specificata, deve essere disabilitata la modifica.
        /// </summary>
        /// <param name="typeFullName">Il nome intero dell'entità che espone la proprietà.</param>
        /// <param name="propertyName">Il nome della proprietà.</param>
        /// <returns>True se la proprietà è esclusa dalle modifiche, altrimenti False.</returns>
        private bool IsExcludedProperty(string typeFullName, string propertyName)
        {
            if (!DeltaCache.excludedProperties.ContainsKey(typeFullName)) return false;
            if (DeltaCache.excludedProperties[typeFullName].Contains(propertyName)) return true;
            return false;
        }

        /// <summary>
        /// Restituisce il tipo specificato dal parametro o il tipo sottostante se <paramref name="type"/> è <see cref="Nullable"/>.
        /// </summary>
        /// <param name="type">Il tipo da verificare.</param>
        /// <returns>Il tipo specificato dal parametro o il tipo sottostante se <paramref name="type"/> è <see cref="Nullable"/>.</returns>
        private Type GetTrueType(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        #region Implementazione dell'interfaccia IDictionary
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

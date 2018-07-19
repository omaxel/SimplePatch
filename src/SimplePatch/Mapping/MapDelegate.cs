using System;

namespace SimplePatch.Mapping
{
    /// <summary>
    /// A delegate for a mapping function which processes the new value of the entities' properties.
    /// </summary>
    /// <param name="propertyType">The type of the property to which assign the value.</param>
    /// <param name="newValue">The new value which can be assigned to the property.</param>
    /// <returns></returns>
    public delegate MapResult<T> MapDelegate<T>(Type propertyType, object newValue);
}

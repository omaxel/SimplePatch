namespace SimplePatch.Mapping
{
    /// <summary>
    /// The result of a <see cref="MapDelegate"/> function execution.
    /// </summary>
    public class MapResult<T>
    {
        /// <summary>
        /// Whatever to skip the current mapping function and execute the default one.
        /// </summary>
        public bool Skip { get; set; } = false;

        /// <summary>
        /// The value to assign to the property when <see cref="Skip"/> is false.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Assigns false to <see cref="Skip"/> and return the current instance.
        /// Use it as a shortcut of 
        /// <code>
        /// instance.Skip = false;
        /// return instance;
        /// </code>
        /// </summary>
        /// <returns></returns>
        public MapResult<T> SkipMap()
        {
            Skip = true;
            return this;
        }

        public static explicit operator MapResult<object>(MapResult<T> v)
        {
            return new MapResult<object>() {
                Skip = v.Skip,
                Value = v.Value
            };
        }
    }
}

using System.Collections.Generic;

namespace SimplePatch
{
    public class DeltaCollection<TEntity> : List<Delta<TEntity>> where TEntity : class, new() { }
}

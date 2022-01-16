using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplePatch.Mapping
{
    public interface IEntityTypeConfiguration<TEntity> where TEntity : class, new()
    {
        void Configuration(DeltaConfig.EntityConfig<TEntity> entityConfig);
    }
}

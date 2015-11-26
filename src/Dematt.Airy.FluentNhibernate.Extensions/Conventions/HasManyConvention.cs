using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the has many convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class HasManyConvention : IHasManyConvention
    {
        /// <summary>
        /// Applies this has many convention to the specified <see cref = "IOneToManyCollectionInstance" />.
        /// </summary>
        /// <param name="instance">An <see cref = "IOneToManyCollectionInstance" />.</param>
        public void Apply(IOneToManyCollectionInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Key.Column(instance.EntityType.Name + "Id");
            instance.Cascade.AllDeleteOrphan();
            instance.Inverse();
        }
    }
}
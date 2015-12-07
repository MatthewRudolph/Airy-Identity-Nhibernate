using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the has many to many convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class HasManyToManyConvention : IHasManyToManyConvention
    {
        /// <summary>
        /// Applies this has many to many convention to the specified <see cref = "IManyToManyCollectionInstance" />.
        /// </summary>
        /// <param name="instance">An <see cref = "IManyToManyCollectionInstance" />.</param>
        public void Apply(IManyToManyCollectionInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Key.Column(instance.EntityType.Name + "Id");
            instance.Relationship.Column(instance.ChildType.Name + "Id");
            instance.AsBag();
            instance.Cascade.All();

            if (instance.OtherSide == null)
            {
                return;
            }

            instance.OtherSide.Key.Column(instance.ChildType.Name + "Id");
            instance.OtherSide.Relationship.Column(instance.EntityType.Name + "Id");
            instance.OtherSide.AsBag();
            instance.OtherSide.Cascade.All();
        }
    }
}
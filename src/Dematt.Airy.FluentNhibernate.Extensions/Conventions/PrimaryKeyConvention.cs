using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the primary key convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class PrimaryKeyConvention : IIdConvention
    {
        /// <summary>
        /// Applies this primary key convention to the specified <see cref = "IIdentityInstance" />.
        /// </summary>
        /// <param name="instance">An <see cref = "IIdentityInstance" />.</param>
        public void Apply(IIdentityInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Column(instance.EntityType.Name + "Id");
        }
    }
}
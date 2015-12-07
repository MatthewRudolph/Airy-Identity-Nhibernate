using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the reference convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class ReferenceConvention : IReferenceConvention
    {
        /// <summary>
        /// Applies this reference convention to the specified <see cref = "IManyToOneInstance" />.
        /// </summary>
        /// <param name="instance">An <see cref = "IManyToOneInstance" />.</param>
        public void Apply(IManyToOneInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Column(instance.Property.Name + "Id");
        }
    }
}
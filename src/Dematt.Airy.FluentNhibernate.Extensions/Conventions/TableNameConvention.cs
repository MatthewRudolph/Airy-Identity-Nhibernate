using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using Inflector;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the table name convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class TableNameConvention : IClassConvention
    {
        /// <summary>
        /// Applies this table name convention to the specified <see cref = "IClassInstance" />.
        /// </summary>
        /// <param name="instance">An <see cref = "IClassInstance" />.</param>
        public void Apply(IClassInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Table(instance.EntityType.Name.Pluralize());
        }
    }
}
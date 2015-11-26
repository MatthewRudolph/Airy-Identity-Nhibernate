using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the column name convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class ColumnNameConvention : IPropertyConvention
    {
        /// <summary>
        /// Applies this column name convention to the specified <see cref = "IPropertyInstance" />.
        /// </summary>
        /// <param name="instance">An <see cref = "IPropertyInstance" />.</param>
        public void Apply(IPropertyInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Column(instance.Property.Name);
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Convention that uses the <see cref="RequiredAttribute"/> set on a property to set the column to not except null values.
    /// </summary>
    public class RequiredPropertyConvention : AttributePropertyConvention<RequiredAttribute>
    {
        /// <summary>
        /// Applies this required convention to the specified <see cref = "IPropertyInstance" />
        /// </summary>
        /// <param name="attribute">A <see cref="RequiredAttribute"/> instance.</param>
        /// <param name="instance">An <see cref = "IPropertyInstance" />.</param>
        protected override void Apply(RequiredAttribute attribute, IPropertyInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Not.Nullable();
        }
    }
}

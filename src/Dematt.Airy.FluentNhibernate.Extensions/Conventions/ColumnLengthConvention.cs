using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Convention that uses the <see cref="StringLengthAttribute"/> MaximumLength property set on a property to set the column length.
    /// </summary>
    public class ColumnLengthConvention : AttributePropertyConvention<StringLengthAttribute>
    {
        /// <summary>
        /// Applies this column length convention to the specified <see cref = "IPropertyInstance" />
        /// </summary>
        /// <param name="attribute">A <see cref="StringLengthAttribute"/> instance.</param>
        /// <param name="instance">An <see cref = "IPropertyInstance" />.</param>
        protected override void Apply(StringLengthAttribute attribute, IPropertyInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            int length = attribute.MaximumLength <= 0 ? 255 : attribute.MaximumLength;
            instance.Length(length);
        }
    }
}

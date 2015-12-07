using Dematt.Airy.Core.Attributes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    ///   Convention that uses the <see cref="IndexAttribute"/> set on a property to create indexes in the database.
    /// </summary>
    public class IndexConvention : AttributePropertyConvention<IndexAttribute>
    {
        /// <summary>
        /// Applies this index convention to the specified <see cref = "IPropertyInstance" />
        /// </summary>
        /// <param name="attribute">A <see cref="IndexAttribute"/> instance.</param>
        /// <param name="instance">An <see cref = "IPropertyInstance" />.</param>
        protected override void Apply(IndexAttribute attribute, IPropertyInstance instance)
        {
            if (attribute.Unique)
            {
                instance.UniqueKey(attribute.Name);
            }
            else
            {
                instance.Index(attribute.Name);
            }
        }
    }
}

using System;
using Dematt.Airy.Nhibernate.Extensions.UserTypes;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies the property convention used by NHibernate for mapping properties with a type of DateTimeOffset to a database schema.
    /// </summary>
    /// <remarks>
    /// This can be used to allow database such as SQLite that do not natively support the DateTimeOffset data type
    /// to store it in two separate columns one for the date and time and another for the offset.
    /// </remarks>
    public class DateTimeOffsetSplitTypeConvention : IPropertyConvention, IPropertyConventionAcceptance
    {
        /// <summary>
        /// Provides the criteria that defines which properties this convention will apply to.
        /// </summary>
        /// <param name="criteria">The object used to define the criteria.</param>
        public void Accept(IAcceptanceCriteria<IPropertyInspector> criteria)
        {
            criteria.Expect(x => x.Type == typeof(DateTimeOffset) || x.Type == typeof(DateTimeOffset?));
        }

        /// <summary>
        /// The convention that is applied to a <see cref = "IPropertyInstance" /> when the criteria from the Accept method is meet.
        /// </summary>
        /// <param name="instance">An <see cref = "IPropertyInstance" /> to apply the convention to.</param>
        public void Apply(IPropertyInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.CustomType<DateTimeOffsetSplitType>();
        }
    }
}

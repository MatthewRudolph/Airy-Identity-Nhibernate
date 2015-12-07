using System;
using System.ComponentModel.DataAnnotations;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.AcceptanceCriteria;
using FluentNHibernate.Conventions.Inspections;
using FluentNHibernate.Conventions.Instances;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Applies the Required Attribute to References as it is not applied by the <see cref="RequiredPropertyConvention"/>
    /// </summary>
    public class RequiredReferenceConvention : IReferenceConvention, IReferenceConventionAcceptance
    {
        /// <summary>
        /// Provides the criteria that defines which properties this convention will apply to.
        /// </summary>
        /// <param name="criteria">The object used to define the criteria.</param>
        public void Accept(IAcceptanceCriteria<IManyToOneInspector> criteria)
        {
            criteria.Expect(x => x.Property.MemberInfo.IsDefined(typeof(RequiredAttribute), false));
        }

        /// <summary>
        /// The convention that is applied to a <see cref = "IPropertyInstance" /> when the criteria from the Accept method is meet.
        /// </summary>
        /// <param name="instance">An <see cref = "IPropertyInstance" /> to apply the convention to.</param>
        public void Apply(IManyToOneInstance instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            instance.Not.Nullable();
        }
    }
}

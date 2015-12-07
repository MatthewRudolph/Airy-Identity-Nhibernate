using System.ComponentModel.DataAnnotations.Schema;
using FluentNHibernate.Automapping;

namespace Dematt.Airy.FluentNhibernate.Extensions
{
    /// <summary>
    /// A default configuration for the Airy implementation of auto mapping using FluentNHibernate.
    /// </summary>
    /// <remarks>
    /// This adds support for the extra attributes such as the <see cref="NotMappedAttribute"/>.
    /// </remarks>
    public class StandardConfiguration : DefaultAutomappingConfiguration
    {
        /// <summary>
        /// Overrides the default ShouldMap method to prevent members that have <see cref="NotMappedAttribute"/> being mapped.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>true if the member should be mapped; false if it should not.</returns>
        public override bool ShouldMap(FluentNHibernate.Member member)
        {
            var array = member.MemberInfo.GetCustomAttributes(typeof(NotMappedAttribute), false);
            bool shouldMap = array.Length == 0;
            return base.ShouldMap(member) && shouldMap;
        }
    }
}

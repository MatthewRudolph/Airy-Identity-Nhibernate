using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Domain
{
    /// <summary>
    /// Other domain entity for tests.
    /// </summary>
    public class OtherDomainObject : EntityWithIntId
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }
    }
}

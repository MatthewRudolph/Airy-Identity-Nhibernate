using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Domain
{
    /// <summary>
    /// Base domain entity for tests.
    /// </summary>
    public abstract class BaseDomainObject : EntityWithIntId
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name { get; set; }
    }
}

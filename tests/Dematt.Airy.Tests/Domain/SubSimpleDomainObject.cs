using System;

namespace Dematt.Airy.Tests.Domain
{
    /// <summary>
    /// Sub Simple domain entity for tests, inherits from <see cref="SimpleDomainObject"/>
    /// </summary>
    public class SubSimpleDomainObject : SimpleDomainObject
    {
        public virtual DateTimeOffset? TestDateTimeOffsetField { get; set; }
    }
}

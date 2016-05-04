using System;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Identity.Entities
{
    /// <summary>
    /// Test Address class for testing One To Many mapping conventions.
    /// </summary>
    public class TestAddress : EntityWithId<Guid>
    {
        public virtual string Line1 { get; set; }
    }
}

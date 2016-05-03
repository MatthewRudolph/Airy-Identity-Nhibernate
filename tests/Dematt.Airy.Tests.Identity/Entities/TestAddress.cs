using System;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Identity.Entities
{
    public class TestAddress : EntityWithId<Guid>
    {
        public virtual string Line1 { get; set; }
    }
}

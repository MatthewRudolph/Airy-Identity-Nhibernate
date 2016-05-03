using System;
using System.ComponentModel.DataAnnotations;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Identity.Entities
{
    public class TestCar : EntityWithId<Guid>
    {
        [StringLength(8)]
        public virtual string RegistrationNumber { get; set; }
    }
}

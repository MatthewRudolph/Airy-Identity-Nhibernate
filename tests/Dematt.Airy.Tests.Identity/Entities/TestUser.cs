// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable UnusedTypeParameter

using System;
using System.ComponentModel.DataAnnotations;
using Dematt.Airy.Core.Attributes;
using Dematt.Airy.Identity;
using Dematt.Airy.Identity.Nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;

namespace Dematt.Airy.Tests.Identity.Entities
{
    public class TestUser : IdentityUser<string, TestLogin, TestRole, string, TestClaim>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser"/> class.
        /// </summary>
        public TestUser()
        {
            Id = Guid.NewGuid().ToString();
            SetRequiredUniqueValues();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser"/> class with the given username.
        /// </summary>
        /// <param name="userName">A username for the user.</param>
        public TestUser(string userName)
            : this()
        {
            UserName = userName;
            SetRequiredUniqueValues();
        }

        private void SetRequiredUniqueValues()
        {
            TestIndex2 = Guid.NewGuid().ToString("B");
            TestIndex5 = Guid.NewGuid().ToString("B");
        }

        public virtual string Hometown { get; set; }

        [StringLength(38)]
        [Index("IX1")]
        public virtual string TestIndex1 { get; set; }

        [StringLength(38)]
        [Index("IU1", Unique = true)]
        public virtual string TestIndex2 { get; set; }

        [StringLength(38)]
        [Index("IX2")]
        public virtual string TestIndex3 { get; set; }

        [StringLength(38)]
        [Index("IX2")]
        [Index("IU2", Unique = true)]
        public virtual string TestIndex4 { get; set; }

        [StringLength(38)]
        [Index("IU2", Unique = true)]
        public virtual string TestIndex5 { get; set; }
    }

    public class TestRole : IdentityRole<TestUser, string>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityRole"/> class.
        /// </summary>
        public TestRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityRole"/> class.
        /// </summary>
        /// <param name="roleName">The name for the role.</param>
        public TestRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    public class TestLogin : IdentityUserLogin<TestUser>
    {

    }

    public class TestClaim : IdentityUserClaim<TestUser, int>
    {

    }

    public class TestUserStore<TUser> : UserStore<TestUser, string, TestLogin, TestRole, string, TestClaim, int>,
        IUserStore<TestUser>
        where TUser : TestUser
    {
        public TestUserStore(ISession context)
            : base(context)
        {
        }
    }

    public class TestRoleStore<TRole> : RoleStore<TestRole, string, TestUser>
        where TRole : TestRole, new()
    {
        public TestRoleStore(ISession context)
            : base(context)
        {
        }
    }
}
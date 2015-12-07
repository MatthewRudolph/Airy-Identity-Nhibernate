// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable UnusedTypeParameter

using System;
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
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser"/> class with the given username.
        /// </summary>
        /// <param name="userName">A username for the user.</param>
        public TestUser(string userName)
            : this()
        {
            UserName = userName;
        }

        public virtual string Hometown { get; set; }
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
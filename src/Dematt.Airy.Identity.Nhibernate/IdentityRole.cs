// ReSharper disable DoNotCallOverridableMethodsInConstructor

using System;
using System.Collections.Generic;
using Dematt.Airy.Core;
using Dematt.Airy.Identity.Nhibernate.Contracts;

namespace Dematt.Airy.Identity.Nhibernate
{
    /// <summary>
    /// The default Airy NHibernate implementation of the ASP.NET Identity IRole interface, using a string for the unique key.
    /// </summary>
    public class IdentityRole : IdentityRole<IdentityUser, string>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityRole"/> class.
        /// </summary>
        public IdentityRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityRole"/> class.
        /// </summary>
        /// <param name="roleName">The name for the role.</param>
        public IdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    /// <summary>
    /// The base Airy NHibernate implementation of the ASP.NET Identity IRole interface.
    /// </summary>
    /// <typeparam name="TUser">The type to use for the User.</typeparam>
    /// <typeparam name="TRoleKey">The type to use for the IRole Id field.</typeparam>
    public abstract class IdentityRole<TUser, TRoleKey> : EntityWithId<TRoleKey>, IIdentityRole<TUser, TRoleKey>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityRole{TUser, TRoleKey}"/> class.
        /// </summary>
        protected IdentityRole()
        {
            Users = new List<TUser>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityRole{TUser, TRoleKey}"/> class.
        /// </summary>
        /// <param name="roleName">The name for the role.</param>
        protected IdentityRole(string roleName)
            : this()
        {
            Name = roleName;
        }

        /// <summary>
        /// The role's name.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// The users that have this role.
        /// </summary>
        public virtual ICollection<TUser> Users { get; protected set; }
    }
}

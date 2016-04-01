// ReSharper disable DoNotCallOverridableMethodsInConstructor

using System;
using System.Collections.Generic;
using Dematt.Airy.Core;
using Dematt.Airy.Identity.Contracts;
using Microsoft.AspNet.Identity;

namespace Dematt.Airy.Identity
{
    /// <summary>
    /// The default Airy NHibernate implementation of the ASP.NET Identity IUser interface, using a string for the unique key.
    /// </summary>
    public class IdentityUser : IdentityUser<string, IdentityUserLogin, IdentityRole, string, IdentityUserClaim>, IUser
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser"/> class.
        /// </summary>
        public IdentityUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser"/> class with the given username.
        /// </summary>
        /// <param name="userName">A username for the user.</param>
        public IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }
    }

    /// <summary>
    /// The base Airy NHibernate implementation of the ASP.NET Identity IUser interface.
    /// </summary>
    /// <typeparam name="TUserKey">The type to use for the IdentityUser Id field.</typeparam>
    /// <typeparam name="TLogin">The type to use for the Logins collection.</typeparam>
    /// <typeparam name="TRole">The type to use for the Roles collection.</typeparam>
    /// <typeparam name="TRoleKey">The type to use for the IRole Id field.</typeparam>
    /// <typeparam name="TClaim">The type to use for the Claims collection.</typeparam>
    public abstract class IdentityUser<TUserKey, TLogin, TRole, TRoleKey, TClaim> : EntityWithId<TUserKey>,
        IIdentityUser<TUserKey, TLogin, TRole, TRoleKey, TClaim>
        where TRole : IRole<TRoleKey>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser{TUserKey,TLogin,TRole,TRoleKey,TClaim}"/> class.
        /// </summary>
        protected IdentityUser()
        {
            Claims = new List<TClaim>();
            Roles = new List<TRole>();
            Logins = new List<TLogin>();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentityUser{TUserKey,TLogin,TRole,TRoleKey,TClaim}"/> class.
        /// </summary>
        /// <param name="userName">A username for the user.</param>
        protected IdentityUser(string userName)
            : this()
        {
            UserName = userName;
        }

        /// <summary>
        /// The user's user name.
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// The user's email address.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// True if the email has been confirmed, false if it hasn't. The default is false.
        /// </summary>
        public virtual bool EmailConfirmed { get; set; }

        /// <summary>
        /// The salted/hashed form of the user's password.
        /// </summary>
        public virtual string PasswordHash { get; set; }

        /// <summary>
        /// A random value that should change whenever the user's credentials have changed (password changed, login removed, etc).
        /// </summary>
        public virtual string SecurityStamp { get; set; }

        /// <summary>
        /// The user's phone number.
        /// </summary>
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// True if the phone number has been confirmed, false if it hasn't. The default is false.
        /// </summary>
        public virtual bool PhoneNumberConfirmed { get; set; }

        /// <summary>
        /// True if two factor authentication is enabled for this user, otherwise false.
        /// </summary>
        public virtual bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// The date and time in UTC when the lockout ends, any time in the past is considered not locked out.
        /// </summary>
        public virtual DateTime? LockoutEndDateUtc { get; set; }

        /// <summary>
        /// True if lockout is enabled for this user.
        /// </summary>
        public virtual bool LockoutEnabled { get; set; }

        /// <summary>
        /// The current number of failed authentication attempts, used for the purposes of user account lockout.
        /// </summary>
        public virtual int AccessFailedCount { get; set; }

        /// <summary>
        /// The roles that this user has.
        /// </summary>
        public virtual ICollection<TRole> Roles { get; protected set; }

        /// <summary>
        /// The claims that this user has.
        /// </summary>
        public virtual ICollection<TClaim> Claims { get; protected set; }

        /// <summary>
        /// The logins that this user has.
        /// </summary>
        public virtual ICollection<TLogin> Logins { get; protected set; }
    }
}

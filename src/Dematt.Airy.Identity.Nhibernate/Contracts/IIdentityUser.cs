using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Dematt.Airy.Identity.Nhibernate.Contracts
{
    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users information using NHibernate.
    /// </summary>
    public interface IIdentityUser<out TUserKey, TLogin, TRole, out TRoleKey, TClaim> : IUser<TUserKey>
        where TRole : IRole<TRoleKey>
    {
        /// <summary>
        /// The user's email address.
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// The roles that this user has.
        /// </summary>
        ICollection<TRole> Roles { get; }

        /// <summary>
        /// The claims that this user has.
        /// </summary>
        ICollection<TClaim> Claims { get; }

        /// <summary>
        /// The logins that this user has.
        /// </summary>
        ICollection<TLogin> Logins { get; }
    }
}
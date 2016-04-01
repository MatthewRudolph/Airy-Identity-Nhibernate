using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace Dematt.Airy.Identity.Contracts
{
    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users role information using NHibernate.
    /// </summary>
    public interface IIdentityRole<TUser, out TRoleKey> : IRole<TRoleKey>
    {
        /// <summary>
        /// The users that have this role.
        /// </summary>
        ICollection<TUser> Users { get; }
    }
}
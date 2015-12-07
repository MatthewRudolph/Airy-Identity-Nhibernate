using Dematt.Airy.Core;
using Dematt.Airy.Identity.Nhibernate.Contracts;

namespace Dematt.Airy.Identity.Nhibernate
{
    /// <summary>
    /// The default Airy NHibernate entity that represents one specific user claim, for users that have a string based key.
    /// </summary>
    public class IdentityUserClaim : IdentityUserClaim<IdentityUser, int>
    {
    }

    /// <summary>
    /// The base Airy NHibernate entity that represents one specific user claim.
    /// </summary>
    /// <typeparam name="TUser">The type to use for the User.</typeparam>
    /// <typeparam name="TClaimKey">The type to use for the IdentityUserClaim Id field.</typeparam>
    public abstract class IdentityUserClaim<TUser, TClaimKey> : EntityWithId<TClaimKey>, IIdentityUserClaim<TUser, TClaimKey>
    {
        /// <summary>
        /// The type of the claim.
        /// </summary>
        public virtual string ClaimType { get; set; }

        /// <summary>
        /// The value of the claim.
        /// </summary>
        public virtual string ClaimValue { get; set; }

        /// <summary>
        /// The user that this claim belongs to.
        /// </summary>
        public virtual TUser User { get; set; }
    }
}

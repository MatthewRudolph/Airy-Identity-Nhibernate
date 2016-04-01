namespace Dematt.Airy.Identity.Contracts
{
    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users login information using NHibernate.
    /// </summary>
    public interface IIdentityUserLogin<TUser>
    {
        /// <summary>
        /// The login provider for the login (i.e. Facebook, Google).
        /// </summary>
        string LoginProvider { get; set; }

        /// <summary>
        /// A key representing the login for the provider.
        /// </summary>
        string ProviderKey { get; set; }

        /// <summary>
        /// The user who owns this login.
        /// </summary>
        TUser User { get; set; }
    }
}
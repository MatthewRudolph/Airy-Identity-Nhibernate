namespace Dematt.Airy.Identity.Contracts
{
    /// <summary>
    /// Interface that defines the minimal set of data required to persist a users claim information using NHibernate.
    /// </summary>
    public interface IIdentityUserClaim<TUser, out TClaimKey>
    {
        /// <summary>
        /// Gets or sets the entity's unique identifier
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The ID may be of type <c>string</c>, <c>int</c>, a custom type, etc.
        ///         The setter is protected to allow unit tests to set this property via reflection
        ///         and to allow domain objects more flexibility in setting this for those objects
        ///         with assigned IDs. It's virtual to allow NHibernate-backed objects to be lazily
        ///         loaded.
        ///     </para>
        /// </remarks>  
        TClaimKey Id { get; }

        /// <summary>
        /// The type of the claim.
        /// </summary>
        string ClaimType { get; set; }

        /// <summary>
        /// The value of the claim.
        /// </summary>
        string ClaimValue { get; set; }

        /// <summary>
        /// The user that this claim belongs to.
        /// </summary>
        TUser User { get; set; }
    }
}
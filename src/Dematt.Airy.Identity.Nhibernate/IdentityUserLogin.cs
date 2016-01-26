using System.Diagnostics.CodeAnalysis;
using Dematt.Airy.Identity.Nhibernate.Contracts;

namespace Dematt.Airy.Identity.Nhibernate
{
    /// <summary>
    /// The Airy NHibernate entity for a user's login. (i.e. Facebook, Google), for users that have a string based key.
    /// </summary>
    public class IdentityUserLogin : IdentityUserLogin<IdentityUser>
    {
    }

    /// <summary>
    /// The Airy NHibernate entity for a user's login. (i.e. Facebook, Google).
    /// </summary>
    /// <typeparam name="TUser">The type used for the user property.</typeparam>
    public abstract class IdentityUserLogin<TUser> : IIdentityUserLogin<TUser>
    {
        /// <summary>
        /// Stores hash code the first time it is calculated.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The hash code is cached because a requirement of a hash code is that
        ///         it does not change once calculated. For example, if this entity was
        ///         added to a hashed collection when transient and then saved, we need
        ///         the same hash code or else it could get lost because it would no 
        ///         longer live in the same bin.
        ///     </para>
        /// </remarks>
        private int? _cachedHashCode;

        /// <summary>
        /// The login provider for the login (i.e. Facebook, Google).
        /// </summary>
        public virtual string LoginProvider { get; set; }

        /// <summary>
        /// A key representing the login for the provider.
        /// </summary>
        public virtual string ProviderKey { get; set; }

        /// <summary>
        /// The user who owns this login.
        /// </summary>
        public virtual TUser User { get; set; }

        public override bool Equals(object obj)
        {
            return ValueEquals(obj);
        }

        /// <summary>
        /// Compare this object to another object.
        /// Returns true when:
        ///   The two objects are of exactly the same type or one is in the inheritance hierarchy of the other AND all of their properties are equal.
        ///   The two objects are actually both the same object.  (i.e. They point to the same reference.)
        ///   The two objects are both null.
        /// Otherwise we return false.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>true or false as detailed by the rules in the summary.</returns>        
        protected bool ValueEquals(object other)
        {
            // If other is null or not an instance of this type then return false.
            if (other == null || !GetType().IsInstanceOfType(other))
            {
                return false;
            }

            // If both entities point to the same reference they must be equal.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // OK if we get here then we need to check the values that make up object.
            var obj = other as IdentityUserLogin<TUser>;
            if (obj == null)
            {
                return false;
            }
            if (LoginProvider == obj.LoginProvider
                && ProviderKey == obj.ProviderKey
                && User.Equals(obj.User))
            {
                return true;
            }

            return false;
        }

        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Thanks R#, but In this case this is the only place we set it and we only set it once.")]
        [SuppressMessage("ReSharper", "ArrangeRedundantParentheses", Justification = "Makes the code more readable to have them.")]
        public override int GetHashCode()
        {
            if (_cachedHashCode != null)
            {
                return _cachedHashCode.Value;
            }

            // Ref: http://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            // Overflow is fine just wrap.
            unchecked
            {
                const int primeToMinimiseCollisions = 486187739;
                int loginProviderHash = LoginProvider == null ? 0 : LoginProvider.GetHashCode();
                int providerKeyHash = ProviderKey == null ? 0 : ProviderKey.GetHashCode();
                int userHash = User == null ? 0 : User.GetHashCode();
                int hash = 17;
                hash = (hash * primeToMinimiseCollisions) + loginProviderHash;
                hash = (hash * primeToMinimiseCollisions) + providerKeyHash;
                hash = (hash * primeToMinimiseCollisions) + userHash;
                _cachedHashCode = hash;

                return _cachedHashCode.Value;
            }
        }
    }
}

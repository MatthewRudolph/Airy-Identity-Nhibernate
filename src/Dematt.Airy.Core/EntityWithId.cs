using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Dematt.Airy.Core
{
    /// <summary>
    /// Base class for entities that have a unique Id.
    /// </summary>
    /// <typeparam name="TId">The type to be used for the Id field.</typeparam>
    public abstract class EntityWithId<TId>
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
        public virtual TId Id { get; protected set; }

        /// <summary>
        /// Sets the Id of the entity.
        /// </summary>
        /// <param name="id">The value to set the Id to.</param>
        public virtual void SetId(TId id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets if the entity is transient or not, i.e. has it ever been saved to the database.
        /// </summary>
        /// <returns>True if the entity is transient; otherwise false.</returns>
        public virtual bool IsTransient()
        {
            // If the Id value is the default for its type then we consider the entity as transient.
            return EqualityComparer<TId>.Default.Equals(Id, default(TId));
        }

        /// <summary>
        /// We override the default implementation of Equals to return true when:
        ///   The two objects are of exactly the same type or one is in the inheritance hierarchy of the other AND their Id's are the same but not the default Id value.
        ///   The two objects are actually both the same object.  (i.e. They point to the same reference.)
        ///   The two objects are both null.
        /// Otherwise we return false.
        /// </summary>
        /// <param name="obj">The object to compare to.</param>
        /// <returns>true or false as detailed by the rules in the summary.</returns>
        public override bool Equals(object obj)
        {
            return EntityEquals(obj as EntityWithId<TId>);
        }

        /// <summary>
        /// We override the default implementation of GetHashCode as we have overridden the default equals implementation.
        /// We use the same logic as for equals:
        ///     If the object is transient we just return base.GetHashCode.
        ///     If it is not transient we return Id.GetHashCode as this is data that our equals logic is based on.
        /// </summary>
        /// <returns>An int that will not change during the lifetime of this object.</returns>        
        [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode", Justification = "Thanks R#, but In this case this is the only place we set it and we only set it once.")]
        [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode", Justification = "Thanks R#, but if the object is transient as we have no safe way to get a hash code so call the default implementation.")]
        public override int GetHashCode()
        {
            if (_cachedHashCode.HasValue)
            {
                return _cachedHashCode.Value;
            }

            _cachedHashCode = IsTransient() ? base.GetHashCode() : Id.GetHashCode();

            return _cachedHashCode.Value;
        }

        /// <summary>
        /// Maintain equality operator semantics for entities.
        /// </summary> 
        public static bool operator ==(EntityWithId<TId> x, EntityWithId<TId> y)
        {
            // By default, == and Equals compares references. In order to 
            // maintain these semantics with entities, we need to compare by 
            // identity value. The Equals(x, y) override is used to guard 
            // against null values; it then calls EntityEquals().
            return Equals(x, y);
        }

        /// <summary>
        /// Maintain inequality operator semantics for entities. 
        /// </summary>
        public static bool operator !=(EntityWithId<TId> x, EntityWithId<TId> y)
        {
            return !(x == y);
        }

        /// <summary>
        /// Compare this object to another <see cref="EntityWithId{TId}"/> object.
        /// Returns true when:
        ///   The two objects are of exactly the same type or one is in the inheritance hierarchy of the other AND their Id's are the same but not the default Id value.
        ///   The two objects are actually both the same object.  (i.e. They point to the same reference.)
        ///   The two objects are both null.
        /// Otherwise we return false.
        /// </summary>
        /// <param name="other">The object to compare to.</param>
        /// <returns>true or false as detailed by the rules in the summary.</returns>       
        protected bool EntityEquals(EntityWithId<TId> other)
        {
            // If other is null or not an instance of this type then return false.
            if (other == null || !GetType().IsInstanceOfType(other))
            {
                return false;
            }

            // If either but not both of the entities is transient (Exclusive OR) return false.
            if (IsTransient() ^ other.IsTransient())
            {
                return false;
            }

            // If both entities are transient then return reference equals.
            if (IsTransient() && other.IsTransient())
            {
                return ReferenceEquals(this, other);
            }

            // OK if we get here then the objects must both be persistent and of the same inheritance hierarchy so compare there Ids.
            // return this.Id == other.Id;
            return Id.Equals(other.Id);
        }
    }
}

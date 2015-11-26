namespace Dematt.Airy.Core
{
    /// <summary>
    ///     Base class for entities that have a unique Id that is an int.
    /// </summary>
    /// <remarks>
    ///     This is just a shortcut for EntityWithId{int}.
    ///     If you want an entity with a identity of a type other than int, 
    ///     such as string, then use <see cref="EntityWithId{TId}" /> instead.
    /// </remarks>
    public abstract class EntityWithIntId : EntityWithId<int>
    {
    }
}

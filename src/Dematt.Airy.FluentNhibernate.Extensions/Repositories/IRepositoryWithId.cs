using System.Linq;
using Dematt.Airy.Core;

namespace Dematt.Airy.FluentNhibernate.Extensions.Repositories
{
    /// <summary>
    /// Interface for creating repositories.
    /// </summary>
    /// <typeparam name="T">The type of object that the repository will persistence.</typeparam>
    /// <typeparam name="TId">The type of the identifier that is used as the unique key for the objects being persisted.</typeparam>
    public interface IRepositoryWithId<T, TId> where T : EntityWithId<TId>
    {
        /// <summary>
        /// Gets a collection of business domain entities of type {T} based on a query defined by the <see cref="IQueryable{T}"/> interface.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> collection of business domain entities.</returns>
        IQueryable<T> All();

        /// <summary>
        /// Updates an existing business entity of type {T}.
        /// </summary>
        /// <param name="entity">The business entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Gets a {T} by Id.
        /// </summary>
        /// <param name="id">The id of {T} to get.</param>
        /// <returns>A business domain entity of type {T}</returns>
        T GetById(TId id);

        /// <summary>
        /// Deletes an existing business entity of type {T}.
        /// </summary>
        /// <param name="entity">The pathways entity to delete.</param>
        void Delete(T entity);

        /// <summary>
        /// Inserts a new business entity of type {T}.
        /// </summary>
        /// <param name="entity">The business entity to insert.</param>
        /// <returns>The inserted business entity of type {T}</returns>
        T Insert(T entity);
    }
}
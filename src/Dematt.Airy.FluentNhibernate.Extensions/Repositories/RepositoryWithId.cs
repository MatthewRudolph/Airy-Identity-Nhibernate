using System.Linq;
using Dematt.Airy.Core;
using NHibernate;
using NHibernate.Linq;

namespace Dematt.Airy.FluentNhibernate.Extensions.Repositories
{
    /// <summary>
    /// Generic repository for use with business entities.
    /// </summary>
    /// <typeparam name="T">The type of entity that the repository will persist.</typeparam>
    /// <typeparam name="TId">The type of the identifier that is used as the unique key for the entities being persisted.</typeparam>
    public class RepositoryWithId<T, TId> : IRepositoryWithId<T, TId> where T : EntityWithId<TId>
    {
        /// <summary>
        /// The NHibernate session used by the repository to perform its data access tasks.
        /// </summary>
        private readonly ISession _session;

        /// <summary>
        /// Initialises a new instance of the <see cref="RepositoryWithId{T,TId}"/> class.
        /// </summary>
        /// <param name="session">An NHibernate session.</param>
        public RepositoryWithId(ISession session)
        {
            _session = session;
        }

        /// <summary>
        /// Gets a collection of business domain entities of type {T} based on a query defined by the <see cref="IQueryable{T}"/> interface.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> collection of business domain entities.</returns>
        public IQueryable<T> All()
        {
            return _session.Query<T>();
        }

        /// <summary>
        /// Updates an existing business entity of type {T}.
        /// </summary>
        /// <param name="entity">The business entity to update.</param>
        public void Update(T entity)
        {
            _session.Update(entity);
        }

        /// <summary>
        /// Gets a {T} by Id.
        /// </summary>
        /// <param name="id">The id of {T} to get.</param>
        /// <returns>A business domain entity of type {T}</returns>
        public T GetById(TId id)
        {
            return _session.Get<T>(id);
        }

        /// <summary>
        /// Deletes an existing business entity of type {T}.
        /// </summary>
        /// <param name="entity">The pathways entity to delete.</param>
        public void Delete(T entity)
        {
            _session.Delete(entity);
        }

        /// <summary>
        /// Inserts a new business entity of type {T}.
        /// </summary>
        /// <param name="entity">The business entity to insert.</param>
        /// <returns>The inserted business entity of type {T}</returns>
        public T Insert(T entity)
        {
            _session.Save(entity);
            return entity;
        }
    }
}

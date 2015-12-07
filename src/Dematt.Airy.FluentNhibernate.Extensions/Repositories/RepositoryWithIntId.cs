using Dematt.Airy.Core;
using NHibernate;

namespace Dematt.Airy.FluentNhibernate.Extensions.Repositories
{
    /// <summary>
    /// Generic NHibernate base repository for use with business entities that have an identity field that is of the type int.
    /// </summary>
    /// <typeparam name="T">The type of entity that the repository will persistence.</typeparam>    
    public class RepositoryWithIntId<T> : RepositoryWithId<T, int>, IRepositoryWithIntId<T> where T : EntityWithIntId
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="RepositoryWithIntId{T}"/> class.
        /// </summary>
        /// <param name="session">An NHibernate session.</param>
        public RepositoryWithIntId(ISession session)
            : base(session)
        {
        }
    }
}

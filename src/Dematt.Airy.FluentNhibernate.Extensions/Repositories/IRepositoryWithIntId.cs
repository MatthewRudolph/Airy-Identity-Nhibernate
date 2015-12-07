using Dematt.Airy.Core;

namespace Dematt.Airy.FluentNhibernate.Extensions.Repositories
{
    /// <summary>
    /// An interface for creating repositories of entities that have an identity field that is of the type int.
    /// </summary>
    /// <typeparam name="T">The type of object that the repository will persist.</typeparam>
    public interface IRepositoryWithIntId<T> : IRepositoryWithId<T, int> where T : EntityWithIntId
    {
    }
}

using Dematt.Airy.FluentNhibernate.Extensions.Repositories;
using Dematt.Airy.Tests.Domain;
using NUnit.Framework;

namespace Dematt.Airy.Tests
{
    /// <summary>
    /// NHibernate persistence tests for Cheaha using SQLite.
    /// </summary>
    [TestFixture]
    public class NHibernatePersistenceTests : BaseInMemoryTest
    {
        /// <summary>
        /// Can we save a <see cref="SimpleDomainObject"/>.
        /// </summary>
        [Test]
        public void PersistSimpleDomainObject()
        {
            var simpleDomainObject = new SimpleDomainObject { Name = "Hello World." };
            var simpleRepository = new RepositoryWithIntId<SimpleDomainObject>(Session);
            using (var transaction = Session.BeginTransaction())
            {
                simpleRepository.Insert(simpleDomainObject);
                transaction.Commit();
            }

            Assert.IsNotNull(simpleDomainObject.Id);
        }

        /// <summary>
        /// Can we save a <see cref="OtherDomainObject"/>.
        /// </summary>
        [Test]
        public void PersistOtherDomainObject()
        {
            var otherDomainObject = new OtherDomainObject { Name = "Hello World." };
            var otherRepository = new RepositoryWithIntId<OtherDomainObject>(Session);
            using (var transaction = Session.BeginTransaction())
            {
                otherRepository.Insert(otherDomainObject);
                transaction.Commit();
            }

            Assert.IsNotNull(otherDomainObject.Id);
        }
    }
}

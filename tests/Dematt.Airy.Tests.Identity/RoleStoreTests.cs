using System.Threading.Tasks;
using Dematt.Airy.Tests.Identity.Entities;
using NHibernate;
using NUnit.Framework;

namespace Dematt.Airy.Tests.Identity
{
    /// <summary>
    /// NHibernate persistence tests for ASP.Net Identity RoleStore using SQLite.
    /// </summary>
    [TestFixture]
    public class RoleStoreTests
    {
        protected static SessionFactoryProvider SessionFactoryProvider;

        protected static ISessionFactory SessionFactory;

        #region Setup and TearDown
        /// <summary>
        /// Sets up the test. Creates an NHibernate Session factory and builds a fresh database schema.
        /// </summary>
        [TestFixtureSetUp]
        public virtual void Setup()
        {
            SessionFactoryProvider = new SessionFactoryProvider();
            SessionFactory = SessionFactoryProvider.DefaultSessionFactory;
            SessionFactoryProvider.BuildSchema();
        }

        /// <summary>
        /// Disposes of the objects once the test has completed.
        /// </summary>
        [TestFixtureTearDown]
        public virtual void TearDown()
        {
            SessionFactory.Dispose();
            SessionFactory = null;
            SessionFactoryProvider = null;
        }
        #endregion

        #region Role Tests
        /// <summary>
        /// Can we create a new role.
        /// </summary>
        [Test]
        public async Task CreateRole()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role.
            var role = new TestRole { Name = "CreateRoleTest" };
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                transaction.Commit();
            }
            // Check the role has an id.
            Assert.IsNotNull(role.Id);

            // Create a new session and role store for this test, so that we actually hit the database and not the cache.
            roleStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            roleStore = new TestRoleStore<TestRole>(session);
            // Load the role.
            TestRole loadRole;
            using (var transaction = session.BeginTransaction())
            {
                loadRole = await roleStore.FindByIdAsync(role.Id);
                transaction.Commit();
            }
            // Check we have the same role.
            Assert.AreEqual(role.Id, loadRole.Id);
            Assert.AreEqual(role.Name, loadRole.Name);
        }

        /// <summary>
        /// Can we get a role by it's Id.
        /// </summary>
        [Test]
        public async Task GetRoleById()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role.
            var role = new TestRole { Name = "GetRoleByIdTest" };
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                transaction.Commit();
            }
            // Check the role has an id.
            Assert.IsNotNull(role.Id);

            // Create a new session and role store for this test, so that we actually hit the database and not the cache.
            roleStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            roleStore = new TestRoleStore<TestRole>(session);
            // Load the role by Id.
            TestRole loadRole;
            using (var transaction = session.BeginTransaction())
            {
                loadRole = await roleStore.FindByIdAsync(role.Id);
                transaction.Commit();
            }
            // Check we have the same role.
            Assert.AreEqual(role.Id, loadRole.Id);
            Assert.AreEqual(role.Name, loadRole.Name);
        }

        /// <summary>
        /// Can we get a role by it's Name.
        /// </summary>
        [Test]
        public async Task GetRoleByName()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role.
            string roleName = "GetRoleByNameTest";
            var role = new TestRole { Name = roleName };
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                transaction.Commit();
            }
            // Check the role has an id.
            Assert.IsNotNull(role.Id);

            // Create a new session and role store for this test, so that we actually hit the database and not the cache.
            roleStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            roleStore = new TestRoleStore<TestRole>(session);
            // Load the role by Name.
            TestRole loadRole;
            using (var transaction = session.BeginTransaction())
            {
                loadRole = await roleStore.FindByNameAsync(roleName);
                transaction.Commit();
            }
            // Check we have the same role.
            Assert.IsNotNull(loadRole);
            Assert.AreEqual(role.Id, loadRole.Id);
            Assert.AreEqual(role.Name, loadRole.Name);
        }

        /// <summary>
        /// Attempting to get a role by id with an id that does not exist should return null.
        /// </summary>
        [Test]
        public async Task GetNonExistingRoleByIdReturnsNull()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            TestRole role;
            using (var transaction = session.BeginTransaction())
            {
                role = await roleStore.FindByIdAsync("THISISNOTAROLEID");
                transaction.Commit();
            }
            // Check that we have no role.
            Assert.IsNull(role);
        }

        /// <summary>
        /// Attempting to get a role by name with an name that does not exist should return null.
        /// </summary>
        [Test]
        public async Task GetNonExistingRoleByNameReturnsNull()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            TestRole role;
            using (var transaction = session.BeginTransaction())
            {
                role = await roleStore.FindByNameAsync("THISISNOTAROLENAME");
                transaction.Commit();
            }
            // Check that we have no role.
            Assert.IsNull(role);
        }

        /// <summary>
        /// Can we update a role.
        /// </summary>
        [Test]
        public async Task UpdateRole()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role.
            string originalRoleName = "UpdateRoleTest";
            string newRoleName = "NewUpdateRoleTest";
            var role = new TestRole { Name = originalRoleName };
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                transaction.Commit();
            }
            // Check the role has an id and a name is correct.
            Assert.IsNotNull(role.Id);
            Assert.AreEqual(role.Name, originalRoleName);
            // Change the role name.
            using (var transaction = session.BeginTransaction())
            {
                role.Name = newRoleName;
                await roleStore.UpdateAsync(role);
                transaction.Commit();
            }

            // Create a new session and role store so that we actually hit the database and not the cache.
            roleStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            roleStore = new TestRoleStore<TestRole>(session);
            // Load the updated the role.
            TestRole updatedRole;
            using (var transaction = session.BeginTransaction())
            {
                updatedRole = await roleStore.FindByIdAsync(role.Id);
                transaction.Commit();
            }

            // Check the role name has been updated and saved.
            Assert.AreEqual(updatedRole.Name, newRoleName);
        }

        /// <summary>
        /// Can we delete a role.
        /// </summary>
        [Test]
        public async Task DeleteRole()
        {
            // Create a session and role store for this test.
            var session = SessionFactory.OpenSession();
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role.
            string userName = "DeleteRoleTest";
            var role = new TestRole { Name = userName };
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                transaction.Commit();
            }
            // Check the role has an id and a name.
            Assert.IsNotNull(role.Id);
            Assert.IsNotNull(role.Name);
            var roleId = role.Id;

            // Create a new session and role store so that we actually hit the database and not the cache.
            roleStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            roleStore = new TestRoleStore<TestRole>(session);
            // Load and delete the role.
            using (var transaction = session.BeginTransaction())
            {
                role = await roleStore.FindByIdAsync(roleId);
                await roleStore.DeleteAsync(role);
                transaction.Commit();
            }

            // Check that the role has been deleted.
            var deletedUser = await roleStore.FindByIdAsync(roleId);
            Assert.IsNull(deletedUser);
        }
        #endregion
    }
}
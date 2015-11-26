using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dematt.Airy.Tests.Identity.Entities;
using Microsoft.AspNet.Identity;
using NHibernate;
using NUnit.Framework;

namespace Dematt.Airy.Tests.Identity
{
    /// <summary>
    /// NHibernate persistence tests for ASP.Net Identity UserStore using SQLite.
    /// </summary>
    [TestFixture]
    public class UserStoreTests
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

        #region User Tests
        /// <summary>
        /// Can we create a new user.
        /// </summary>
        [Test]
        public async Task CreateUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            var user = new TestUser { UserName = "CreateUserTest" };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id.
            Assert.IsNotNull(user.Id);

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(user.Id);
                transaction.Commit();
            }
            // Check we have the same user.
            Assert.AreEqual(user.Id, loadUser.Id);
            Assert.AreEqual(user.UserName, loadUser.UserName);
        }

        /// <summary>
        /// Can we get a user by id.
        /// </summary>
        [Test]
        public async Task GetUserById()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            string userName = "GetUserByUserIdTest";
            var user = new TestUser { UserName = userName };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id and a username.
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UserName);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user.
            Assert.AreEqual(user.Id, loadUser.Id);
            Assert.AreEqual(user.UserName, loadUser.UserName);
        }

        /// <summary>
        /// Can we get a user by id.
        /// </summary>
        [Test]
        public async Task GetUserByIdUsesCache()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            string userName = "GetUserByUserIdUsesCacheTest";
            var user = new TestUser { UserName = userName };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id and a username.
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UserName);
            var userId = user.Id;

            userStore = new TestUserStore<TestUser>(session);
            // Load the user inside the same session, this should use the cache and not hit the database.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user.
            Assert.AreEqual(user.Id, loadUser.Id);
            Assert.AreEqual(user.UserName, loadUser.UserName);
        }

        /// <summary>
        /// Can we get a user by username.
        /// </summary>
        [Test]
        public async Task GetUserByUserName()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            string userName = "GetUserByUserNameTest";
            var user = new TestUser { UserName = userName };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id and a username.
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UserName);

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user using the username.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByNameAsync(userName);
                transaction.Commit();
            }
            // Check we have the same user.
            Assert.AreEqual(user.Id, loadUser.Id);
            Assert.AreEqual(user.UserName, loadUser.UserName);
        }

        /// <summary>
        /// Can we get a user by email address.
        /// </summary>
        [Test]
        public async Task GetUserByEmail()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            string userName = "GetUserByEmailTest";
            string email = "GetUserByEmail@nodom.com";
            var user = new TestUser { UserName = userName, Email = email };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id and a username and email.
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UserName);
            Assert.IsNotNull(user.Email);

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user using the email.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByEmailAsync(email);
                transaction.Commit();
            }
            // Check we have the same user.
            Assert.AreEqual(user.Id, loadUser.Id);
            Assert.AreEqual(user.UserName, loadUser.UserName);
            Assert.AreEqual(user.Email, loadUser.Email);
        }

        /// <summary>
        /// Attempting to get a user by id with an id that does not exist should return null.
        /// </summary>
        [Test]
        public async Task GetNonExistingUserByIdReturnsNull()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            TestUser user;
            using (var transaction = session.BeginTransaction())
            {
                user = await userStore.FindByIdAsync("THISISNOTAUSERID");
                transaction.Commit();
            }
            // Check that we have no user.
            Assert.IsNull(user);
        }

        /// <summary>
        /// Attempting to get a user by username with a username that does not exist should return null.
        /// </summary>
        [Test]
        public async Task GetNonExistingUserByNameReturnsNull()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            TestUser user;
            using (var transaction = session.BeginTransaction())
            {
                user = await userStore.FindByNameAsync("THISISNOTAUSERNAME");
                transaction.Commit();
            }
            // Check that we have no user.
            Assert.IsNull(user);
        }

        /// <summary>
        /// Can we update a user.
        /// </summary>
        [Test]
        public async Task UpdateUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            string userName = "UpdateUserTest";
            string email = "updateusertest@nodom.com";
            var user = new TestUser { UserName = userName };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id and a username.
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UserName);
            Assert.IsNull(user.Email);
            // Update the user's email address.
            using (var transaction = session.BeginTransaction())
            {
                user.Email = email;
                await userStore.UpdateAsync(user);
                transaction.Commit();
            }

            // Create a new session and user store so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load and update the user.
            TestUser updatedUser;
            using (var transaction = session.BeginTransaction())
            {
                updatedUser = await userStore.FindByIdAsync(user.Id);
                transaction.Commit();
            }

            // Check the email has been updated and saved.
            Assert.AreEqual(updatedUser.Email, email);
        }

        /// <summary>
        /// Can we delete a user.
        /// </summary>
        [Test]
        public async Task DeleteUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user.
            string userName = "DeleteUserTest";
            var user = new TestUser { UserName = userName };
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an id and a username.
            Assert.IsNotNull(user.Id);
            Assert.IsNotNull(user.UserName);
            var userId = user.Id;

            // Create a new session and user store so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load and delete the user.
            using (var transaction = session.BeginTransaction())
            {
                user = await userStore.FindByIdAsync(userId);
                await userStore.DeleteAsync(user);
                transaction.Commit();
            }

            // Check that the user has been deleted.
            var deletedUser = await userStore.FindByIdAsync(userId);
            Assert.IsNull(deletedUser);
        }
        #endregion

        #region UserLogin Tests
        /// <summary>
        /// Can we add a new login to a user.
        /// </summary>
        [Test]
        public async Task AddLoginForUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with a login.
            var user = new TestUser { UserName = "AddLoginForUserTest" };
            var login = new UserLoginInfo("TestProviderAdd", "ProviderKeyAdd");
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                await userStore.AddLoginAsync(user, login);
                transaction.Commit();
            }
            // Check the user has an id and the login.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Logins.Count, 1);
        }

        /// <summary>
        /// Can we remove a login from a user.
        /// </summary>
        [Test]
        public async Task RemoveLoginForUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with a login.
            var user = new TestUser { UserName = "RemoveLoginForUserTest" };
            var login = new UserLoginInfo("TestProviderRemove", "ProviderKeyRemove");
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                await userStore.AddLoginAsync(user, login);
                transaction.Commit();
            }
            // Check the user has an id and the login.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Logins.Count, 1);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user and remove the login.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                await userStore.RemoveLoginAsync(loadUser, login);
                transaction.Commit();
            }
            // Check we have the same user and that the login has been removed.
            Assert.AreEqual(loadUser.Id, user.Id);
            Assert.AreEqual(loadUser.Logins.Count, 0);
        }

        /// <summary>
        /// Can we get a list of a users logins.
        /// </summary>
        [Test]
        public async Task GetLoginsForUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with a login.
            var user = new TestUser { UserName = "GetLoginsForUserTest" };
            int numberOfLogins = 5;
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                for (int i = 0; i < numberOfLogins; i++)
                {
                    var login = new UserLoginInfo("TestProviderList" + i, "ProviderKeyRemove" + i);
                    await userStore.AddLoginAsync(user, login);
                }
                transaction.Commit();
            }
            // Check the user has an id and all the logins have been saved.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Logins.Count, numberOfLogins);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            IList<UserLoginInfo> logins;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                logins = await userStore.GetLoginsAsync(user);
                transaction.Commit();
            }
            // Check we have the same user and that they have all of the logins.
            Assert.AreEqual(loadUser.Id, user.Id);
            Assert.AreEqual(loadUser.Logins.Count, numberOfLogins);
            Assert.AreEqual(logins.Count, numberOfLogins);
        }

        /// <summary>
        /// Can we get a user by a user login.
        /// </summary>
        [Test]
        public async Task GetUserByLogin()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with a login.
            var user = new TestUser { UserName = "GetUserByLogin" };
            var login = new UserLoginInfo("TestProviderGetUser", "ProviderKeyGetUser");
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                await userStore.AddLoginAsync(user, login);
                transaction.Commit();
            }
            // Check the user has an id and the login.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Logins.Count, 1);

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindAsync(new UserLoginInfo("TestProviderGetUser", "ProviderKeyGetUser"));
                transaction.Commit();
            }
            // Check we have the same user and it has a single login.
            Assert.AreEqual(loadUser.Id, user.Id);
            Assert.AreEqual(loadUser.Logins.Count, 1);
        }
        #endregion

        #region UserClaims Test
        /// <summary>
        /// Can we add a new claim to a user.
        /// </summary>
        [Test]
        public async Task AddClaimForUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with a claim.
            var user = new TestUser { UserName = "AddClaimForUserTest" };
            var claim = new Claim(ClaimTypes.Role, "Admin_AddClaimForUserTest");
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                await userStore.AddClaimAsync(user, claim);
                transaction.Commit();
            }
            // Check the user has an id and the claim.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Claims.Count, 1);
        }

        /// <summary>
        /// Can we get the claims for a user.
        /// </summary>
        [Test]
        public async Task RemoveClaimForUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with a claim.
            var user = new TestUser { UserName = "RemoveClaimForUserTest" };
            var claimType = ClaimTypes.Role;
            var claimValue = "Admin_RemoveClaimForUserTest";
            var claim = new Claim(claimType, claimValue);
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                await userStore.AddClaimAsync(user, claim);
                transaction.Commit();
            }
            // Check the user has an id and the claim.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Claims.Count, 1);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user and remove the claim.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                await userStore.RemoveClaimAsync(loadUser, claim);
                transaction.Commit();
            }
            // Check we have the same user and it now has no claims.
            Assert.AreEqual(loadUser.Id, user.Id);
            Assert.AreEqual(loadUser.Claims.Count, 0);
        }

        /// <summary>
        /// Can we get the claims for a user.
        /// </summary>
        [Test]
        public async Task GetClaimsForUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            // Create and save a user with some claims.
            var user = new TestUser { UserName = "GetClaimsForUserTest" };
            int numberOfClaims = 5;
            var claimType = ClaimTypes.Role;
            var claimValue = "Admin_GetClaimsForUserTest";
            var claim = new Claim(claimType, claimValue);
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                await userStore.AddClaimAsync(user, claim);
                for (int i = 0; i < numberOfClaims - 1; i++)
                {
                    var loopClaim = new Claim(claimType, "Admin_GetClaimsForUserTest_" + i);
                    await userStore.AddClaimAsync(user, loopClaim);
                }
                transaction.Commit();
            }
            // Check the user has an id and the claims.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Claims.Count, numberOfClaims);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and it has the claims.
            Assert.AreEqual(loadUser.Id, user.Id);
            Assert.AreEqual(loadUser.Claims.Count, numberOfClaims);
            var userClaims = await userStore.GetClaimsAsync(loadUser);
            var userClaim = userClaims.SingleOrDefault(c => c.Type == claimType && c.Value == claimValue);
            Assert.IsNotNull(userClaim);
        }
        #endregion

        #region UserRoles Test
        /// <summary>
        /// Can we add a role to a user.
        /// </summary>
        [Test]
        public async Task AddRoleToUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role and a user.
            string roleName = "AddRoleToUserTestRole";
            var role = new TestRole(roleName);
            var user = new TestUser("AddRoleToUserTestUser");
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                await userStore.CreateAsync(user);
                transaction.Commit();
            }
            // Check the user has an Id and no roles.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Roles.Count, 0);
            var userId = user.Id;
            // Add the user to the role.
            using (var transaction = session.BeginTransaction())
            {
                await userStore.AddToRoleAsync(user, role.Name);
                transaction.Commit();
            }

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and it has the role.
            Assert.AreEqual(loadUser.Id, user.Id);
            var userRole = loadUser.Roles.SingleOrDefault(r => r.Name == roleName);
            Assert.IsNotNull(userRole);
        }

        /// <summary>
        /// Can we remove a role from a user.
        /// </summary>
        [Test]
        public async Task RemoveRoleFromUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role and a user and add the role to the user.
            string roleName = "RemoveRoleFromUserTestRole";
            var role = new TestRole(roleName);
            var user = new TestUser("RemoveRoleFromUser");
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, role.Name);
                transaction.Commit();
            }
            // Check the user has an Id and the role.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Roles.Count, 1);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and it has the role.
            Assert.AreEqual(loadUser.Id, user.Id);
            var userRole = loadUser.Roles.SingleOrDefault(r => r.Name == roleName);
            Assert.IsNotNull(userRole);
            // Now remove the role.
            using (var transaction = session.BeginTransaction())
            {
                await userStore.RemoveFromRoleAsync(loadUser, roleName);
                transaction.Commit();
            }

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user again.            
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and the role has been removed.
            Assert.AreEqual(loadUser.Id, user.Id);
            userRole = loadUser.Roles.SingleOrDefault(r => r.Name == roleName);
            Assert.IsNull(userRole);
        }

        /// <summary>
        /// When a role is removed from a user that has multiple roles only the correct role is removed.
        /// </summary>
        [Test]
        public async Task RemoveRoleFromUserOnlyRemovesSingleRole()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role and a user and add the role to the user.
            int numberOfOtherRoles = 3;
            string roleName = "RemoveRoleFromUserOnlyRemovesSingleRole";
            var role = new TestRole(roleName);
            var user = new TestUser("RemoveRoleFromUserOnlyRemovesSingleRole");
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, role.Name);
                for (int i = 0; i < numberOfOtherRoles; i++)
                {
                    var otherRole = new TestRole(roleName + i);
                    await roleStore.CreateAsync(otherRole);
                    await userStore.AddToRoleAsync(user, otherRole.Name);
                }
                transaction.Commit();
            }
            // Check the user has an Id and the roles.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Roles.Count, numberOfOtherRoles + 1);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and it has the role.
            Assert.AreEqual(loadUser.Id, user.Id);
            var userRole = loadUser.Roles.SingleOrDefault(r => r.Name == roleName);
            Assert.IsNotNull(userRole);
            // Now remove the role.
            using (var transaction = session.BeginTransaction())
            {
                await userStore.RemoveFromRoleAsync(loadUser, roleName);
                transaction.Commit();
            }

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user again.            
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and the role has been removed.
            Assert.AreEqual(loadUser.Id, user.Id);
            userRole = loadUser.Roles.SingleOrDefault(r => r.Name == roleName);
            Assert.IsNull(userRole);
        }

        /// <summary>
        /// Can we add a role to a user.
        /// </summary>
        [Test]
        public async Task GetRolesForAUser()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a user and some roles and add the roles to the user.
            int numberOfRoles = 5;
            string roleName = "GetRolesForAUserTestRole";
            var user = new TestUser("GetRolesForAUser");
            using (var transaction = session.BeginTransaction())
            {
                await userStore.CreateAsync(user);
                for (int i = 0; i < numberOfRoles; i++)
                {
                    var role = new TestRole(roleName + i);
                    await roleStore.CreateAsync(role);
                    await userStore.AddToRoleAsync(user, role.Name);
                }
                transaction.Commit();
            }
            // Check the user has an Id and the roles.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Roles.Count, numberOfRoles);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            IList<string> roles;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                roles = await userStore.GetRolesAsync(user);
                transaction.Commit();
            }
            // Check we have the same user and it has the role.
            Assert.AreEqual(loadUser.Id, user.Id);
            Assert.AreEqual(roles.Count, numberOfRoles);
        }

        /// <summary>
        /// The IsInRoleAsync methods returns true when a user is in role and false when they are not.
        /// </summary>
        [Test]
        public async Task IsInRoleReturnsTrueWhenAUserIsInARoleAndFalseWhenTheyAreNot()
        {
            // Create a session and user store for this test.
            var session = SessionFactory.OpenSession();
            var userStore = new TestUserStore<TestUser>(session);
            var roleStore = new TestRoleStore<TestRole>(session);
            // Create and save a role and a user and add the role to the user.
            int numberOfOtherRoles = 3;
            string roleName = "IsInRoleTestRole";
            var role = new TestRole(roleName);
            var user = new TestUser("IsInRoleTestUser");
            using (var transaction = session.BeginTransaction())
            {
                await roleStore.CreateAsync(role);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, role.Name);
                for (int i = 0; i < numberOfOtherRoles; i++)
                {
                    var otherRole = new TestRole(roleName + i);
                    await roleStore.CreateAsync(otherRole);
                    await userStore.AddToRoleAsync(user, otherRole.Name);
                }
                transaction.Commit();
            }
            // Check the user has an Id and the roles.
            Assert.IsNotNull(user.Id);
            Assert.AreEqual(user.Roles.Count, numberOfOtherRoles + 1);
            var userId = user.Id;

            // Create a new session and user store for this test, so that we actually hit the database and not the cache.
            userStore.Dispose();
            session.Dispose();
            session = SessionFactory.OpenSession();
            userStore = new TestUserStore<TestUser>(session);
            // Load the user.
            TestUser loadUser;
            using (var transaction = session.BeginTransaction())
            {
                loadUser = await userStore.FindByIdAsync(userId);
                transaction.Commit();
            }
            // Check we have the same user and that we get true when testing for the correct role and false for non-existent role.
            Assert.AreEqual(loadUser.Id, user.Id);
            bool inRole = await userStore.IsInRoleAsync(loadUser, roleName);
            bool notInRole = await userStore.IsInRoleAsync(loadUser, "NOTINROLETEST_USERNOTINROLE");
            Assert.IsTrue(inRole);
            Assert.IsFalse(notInRole);
        }
        #endregion
    }
}

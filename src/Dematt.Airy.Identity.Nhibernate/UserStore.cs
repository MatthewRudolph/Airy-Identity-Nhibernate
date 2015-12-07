using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq;

namespace Dematt.Airy.Identity.Nhibernate
{
    public class UserStore<TUser, TUserKey, TLogin, TRole, TRoleKey, TClaim, TClaimKey> :
        IUserLoginStore<TUser, TUserKey>,
        IUserClaimStore<TUser, TUserKey>,
        IUserRoleStore<TUser, TUserKey>,
        IUserPasswordStore<TUser, TUserKey>,
        IUserSecurityStampStore<TUser, TUserKey>,
        IQueryableUserStore<TUser, TUserKey>,
        IUserEmailStore<TUser, TUserKey>,
        IUserPhoneNumberStore<TUser, TUserKey>,
        IUserTwoFactorStore<TUser, TUserKey>,
        IUserLockoutStore<TUser, TUserKey>
        where TUserKey : IEquatable<TUserKey>
        where TUser : IdentityUser<TUserKey, TLogin, TRole, TRoleKey, TClaim>, IUser<TUserKey>
        where TLogin : IdentityUserLogin<TUser>, new()
        where TRole : IdentityRole<TUser, TRoleKey>, new()
        where TClaim : IdentityUserClaim<TUser, TClaimKey>, new()
    {
        /// <summary>
        /// Constructor which takes a <see cref="ISession"/> to use for the context.
        /// </summary>
        public UserStore(ISession context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            DisposeContext = false;
            AutoSaveChanges = false;
        }

        private bool _disposed;

        /// <summary>
        /// Context (NHibernate Session) for the user store.
        /// </summary>
        public ISession Context { get; private set; }

        /// <summary>
        /// If true will call dispose on the Session during Dispose, false means external code is responsible for disposing the session, default is false.
        /// </summary>
        public bool DisposeContext { get; set; }

        /// <summary>
        /// If true changes will be persisted to the database immediately after Create/Update/Delete, default is false.
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        /// <summary>
        /// Returns an IQueryable of Users.
        /// </summary>
        public IQueryable<TUser> Users
        {
            get
            {
                ThrowIfDisposed();
                return Context.Query<TUser>();
            }
        }

        public Task CreateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            Context.Save(user);
            return SaveChanges();
        }

        public Task UpdateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            Context.Update(user);
            return SaveChanges();
        }

        public Task DeleteAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            Context.Delete(user);
            return SaveChanges();
        }

        public Task<TUser> FindByIdAsync(TUserKey userId)
        {
            ThrowIfDisposed();

            // Get method, this allows NHibernate to use it cache when it can.
            return GetUserAggregateByIdAsync(userId);

            // Query method would always hit the database, however...
            // It does not work when we have a string as the generic type for the user id, NHibernate throws a System.NotSupportedException : Boolean Equals(typeof(TUserKey)).
            // Related details in this reference: http://www.primordialcode.com/blog/post/linq-to-nhibernate-string.equals-with-stringcomparison-option/
            // Also == can not be used as the TUserKey generic type could be a struct and structs don't support == by default.
            // return GetUserAggregateAsync(u => u.Id.Equals(userId));
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(userName))
            {
                throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "userName");
            }
            return GetUserAggregateAsync(u => u.UserName == userName);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }
            user.Logins.Add(new TLogin
            {
                User = user,
                ProviderKey = login.ProviderKey,
                LoginProvider = login.LoginProvider
            });
            return SaveChanges();
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin =
                user.Logins.SingleOrDefault(l =>
                    l.LoginProvider == login.LoginProvider &&
                    l.ProviderKey == login.ProviderKey);
            if (userLogin != null)
            {
                user.Logins.Remove(userLogin);
            }
            return SaveChanges();
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var userLogins = user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey)).ToList();
            return Task.FromResult<IList<UserLoginInfo>>(userLogins);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            ThrowIfDisposed();
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var userLogin =
                Context.Query<TLogin>().SingleOrDefault(l =>
                    l.LoginProvider == login.LoginProvider &&
                    l.ProviderKey == login.ProviderKey);
            if (userLogin != null)
            {
                return GetUserAggregateByIdAsync(userLogin.User.Id);
            }
            return null;
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var userClaims = user.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
            return Task.FromResult<IList<Claim>>(userClaims);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            user.Claims.Add(new TClaim
            {
                User = user,
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
            return SaveChanges();
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }
            var userClaims = user.Claims.Where(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type).ToList();
            foreach (var userClaim in userClaims)
            {
                user.Claims.Remove(userClaim);
            }
            return SaveChanges();
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "roleName");
            }
            var role = Context.Query<TRole>().SingleOrDefault(r => r.Name == roleName);
            if (role == null)
            {
                throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, IdentityResources.RoleNotFound, roleName));
            }
            user.Roles.Add(role);
            return SaveChanges();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "roleName");
            }
            var role = user.Roles.SingleOrDefault(r => r.Name == roleName);
            if (role != null)
            {
                user.Roles.Remove(role);
            }
            return SaveChanges();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult((IList<string>)user.Roles.Select(r => r.Name).ToList());
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (String.IsNullOrWhiteSpace(roleName))
            {
                throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "roleName");
            }
            return Task.FromResult(user.Roles.Any(r => r.Name == roleName));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PasswordHash = passwordHash;
            return Task.FromResult(0);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PasswordHash != null);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.SecurityStamp);
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.Email = email;
            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.EmailConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            if (String.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException(IdentityResources.ValueCannotBeNullOrEmpty, "email");
            }
            return GetUserAggregateAsync(u => u.Email == email);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumber = phoneNumber;
            return Task.FromResult(0);
        }

        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.PhoneNumberConfirmed = confirmed;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.TwoFactorEnabled = enabled;
            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEndDateUtc = lockoutEnd == DateTimeOffset.MinValue ? (DateTime?)null : lockoutEnd.UtcDateTime;
            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            ThrowIfDisposed();
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            user.LockoutEnabled = enabled;
            return Task.FromResult(0);
        }

        /// <summary>
        /// Dispose this user store.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// If disposing, calls dispose on the Context.  Always nulls out the Context.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && DisposeContext && Context != null)
            {
                Context.Dispose();
            }
            _disposed = true;
            Context = null;
        }

        /// <summary>
        /// Eagerly loads a user, their roles, claims and logins by Id.
        /// </summary>
        /// <remarks>Uses Get method, this allows NHibernate to use it's cache when it can.</remarks>
        protected Task<TUser> GetUserAggregateByIdAsync(TUserKey userId)
        {
            try
            {
                var user = Context.Get<TUser>(userId);
                if (user != null)
                {
                    NHibernateUtil.Initialize(user.Roles);
                    NHibernateUtil.Initialize(user.Claims);
                    NHibernateUtil.Initialize(user.Logins);
                }
                return Task.FromResult(user);
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<TUser>();
                tcs.SetException(ex);
                return tcs.Task;
            }

            // In this context it's probably best not to use Task.Run but to use Task.FromResult after doing our synchronous work as above becuase...
            // http://blog.stephencleary.com/2013/11/taskrun-etiquette-examples-dont-use.html
            // http://www.ben-morris.com/why-you-shouldnt-create-asynchronous-wrappers-with-task-run/
            //return Task.Run(() =>
            //{
            //    var user = Context.Get<TUser>(userId);
            //    if (user != null)
            //    {
            //        NHibernateUtil.Initialize(user.Roles);
            //        NHibernateUtil.Initialize(user.Claims);
            //        NHibernateUtil.Initialize(user.Logins);
            //    }
            //    return user;
            //});
        }

        /// <summary>
        /// Used to attach child entities to the User aggregate, i.e. Roles, Logins, and Claims.        
        /// </summary>
        /// <remarks>Uses Query method, this means NHibernate will always hit the database.</remarks>
        protected virtual Task<TUser> GetUserAggregateAsync(Expression<Func<TUser, bool>> filter)
        {
            try
            {
                var query = Context.Query<TUser>().Where(filter);
                query.Fetch(p => p.Roles).ToFuture();
                query.Fetch(p => p.Claims).ToFuture();
                query.Fetch(p => p.Logins).ToFuture();
                return Task.FromResult(query.ToFuture().FirstOrDefault());
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<TUser>();
                tcs.SetException(ex);
                return tcs.Task;
            }

            // In this context it's probably best not to use Task.Run but to use Task.FromResult after doing our synchronous work as above becuase...
            // http://blog.stephencleary.com/2013/11/taskrun-etiquette-examples-dont-use.html
            // http://www.ben-morris.com/why-you-shouldnt-create-asynchronous-wrappers-with-task-run/            
            //return Task.Run(() =>
            //{
            //    var query = Context.Query<TUser>().Where(filter);
            //    query.Fetch(p => p.Roles).ToFuture();
            //    query.Fetch(p => p.Claims).ToFuture();
            //    query.Fetch(p => p.Logins).ToFuture();
            //    return query.ToFuture().FirstOrDefault();
            //});
        }

        /// <summary>
        ///  Forces saving changes to the database by calling Flush on the NHibernate session.
        /// </summary>
        private Task SaveChanges()
        {
            if (AutoSaveChanges)
            {
                Context.Flush();
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Throws an error if we have disposed the Context.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
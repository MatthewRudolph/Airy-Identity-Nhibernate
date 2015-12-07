using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NHibernate;
using NHibernate.Linq;

namespace Dematt.Airy.Identity.Nhibernate
{
    public class RoleStore<TRole, TRoleKey, TUser> : IQueryableRoleStore<TRole, TRoleKey>
        where TRole : IdentityRole<TUser, TRoleKey>, new()
    {
        /// <summary>
        /// Constructor which takes a <see cref="ISession"/> to use for the context.
        /// </summary>
        public RoleStore(ISession context)
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
        /// Context (NHibernate Session) for the role store.
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
        /// Returns an IQueryable of Roles.
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get
            {
                ThrowIfDisposed();
                return Context.Query<TRole>();
            }
        }

        public Task CreateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Context.Save(role);
            return SaveChanges();
        }

        public Task UpdateAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Context.Update(role);
            return SaveChanges();
        }

        public Task DeleteAsync(TRole role)
        {
            ThrowIfDisposed();
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }
            Context.Delete(role);
            return SaveChanges();
        }

        public Task<TRole> FindByIdAsync(TRoleKey roleId)
        {
            ThrowIfDisposed();
            try
            {
                if (roleId == null)
                {
                    throw new ArgumentNullException("roleId");
                }
                // Lazy loading is fine here we really only want the Role details, we don't want to be loading all users or users Ids that have the role.
                return Task.FromResult(Context.Get<TRole>(roleId));
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<TRole>();
                tcs.SetException(ex);
                return tcs.Task;
            }
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            try
            {
                return Task.FromResult(Context.Query<TRole>().FirstOrDefault(r => r.Name == roleName));
            }
            catch (Exception ex)
            {
                var tcs = new TaskCompletionSource<TRole>();
                tcs.SetException(ex);
                return tcs.Task;
            }
        }

        /// <summary>
        /// Dispose this role store.
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
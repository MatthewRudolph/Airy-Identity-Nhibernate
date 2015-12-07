using System;
using NHibernate;

namespace Dematt.Airy.Nhibernate.Extensions
{
    /// <summary>
    /// A class the encapsulates an atomic unit of work.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// The NHibernate session used to create transactions.
        /// </summary>
        private readonly ISession _session;

        /// <summary>
        /// The transaction used by the unit if work.
        /// </summary>
        private ITransaction _transaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="session">An object that implements the <see cref="ISession"/> interface.</param>
        public UnitOfWork(ISession session)
        {
            if (_session == null)
            {
                throw new ArgumentNullException("session");
            }
            _session = session;
        }

        /// <summary>
        /// Sets the unit of work to begin.
        /// </summary>
        public void Begin()
        {
            if (_session == null)
            {
                throw new NullReferenceException("There is no session object associated with this unit of work.");
            }
            _transaction = _session.BeginTransaction();
        }

        /// <summary>
        /// Commits the complete unit of work.
        /// </summary>
        public void Commit()
        {
            if (_session == null)
            {
                throw new NullReferenceException("There is no session object associated with this unit of work.");
            }
            if (_transaction == null)
            {
                throw new NullReferenceException("There is no transaction object associated with this unit of work.  The unit of work might not have begun or it may have already been cancelled or committed");
            }

            try
            {
                if (!_transaction.IsActive)
                {
                    return;
                }

                _transaction.Commit();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }

        /// <summary>
        /// Cancels the unit of work ensuring that no changes have been made.
        /// </summary>
        public void Cancel()
        {
            if (_session == null)
            {
                throw new NullReferenceException("There is no session object associated with this unit of work.");
            }

            if (_transaction == null)
            {
                // No transaction to cancel just return.
                return;
            }

            try
            {
                _transaction.Rollback();
            }
            finally
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}

using System;
using NHibernate;

namespace Dematt.Airy.FluentNhibernate.Extensions
{
    /// <summary>
    /// A class to help with the initialising of NHibernate.
    /// </summary>
    /// <remarks>Usually created as a singleton by a IoC container.</remarks>
    public static class NHibernateInitialiser
    {
        /// <summary>
        /// Creates an NHibernate session factory.
        /// </summary>
        /// <returns>A NHibernate session factory</returns>
        /// <param name="sessionFactoryInitialisationMethod">A method that returns a completely configured NHibernate Session Factory.</param>
        public static ISessionFactory BuildSessionFactory(Func<ISessionFactory> sessionFactoryInitialisationMethod)
        {
            return sessionFactoryInitialisationMethod();
        }
    }
}

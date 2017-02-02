using System.Web.Hosting;
using Dematt.Airy.Identity;
using Dematt.Airy.Identity.Nhibernate;
using Dematt.Airy.Sample.IntWebSite.Models;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Dematt.Airy.Sample.IntWebSite
{
    /// <summary>
    /// NHibernate session factory provider we only ever want one of these :-)
    /// </summary>
    public class SessionFactoryProvider
    {
        private readonly Configuration _configuration;

        public SessionFactoryProvider()
        {
            _configuration = new Configuration();
            _configuration.Configure(HostingEnvironment.MapPath("~/Nhibernate.config"));
            CreateNHibernateSessionFactory();
            BuildSchema();
        }

        public SessionFactoryProvider(string nHhibernateConfigFile)
        {
            _configuration = new Configuration();
            _configuration.Configure(nHhibernateConfigFile);
            CreateNHibernateSessionFactory();
            BuildSchema();
        }


        /// <summary>
        /// The NHibernate session factory use to obtain sessions.
        /// </summary>
        public ISessionFactory DefaultSessionFactory;

        /// <summary>
        /// Creates/Updates the database schema.
        /// </summary>
        private void BuildSchema()
        {
            // Build the schema.
            var schemaUpdate = new SchemaUpdate(_configuration);

            // Create/Update the schema.
            schemaUpdate.Execute(false, true);
        }

        /// <summary>
        /// Creates the NHibernare session factory.
        /// </summary>
        private void CreateNHibernateSessionFactory()
        {
            if (DefaultSessionFactory == null)
            {
                var mappingHelper = new MappingHelper<ApplicationUser, int, ApplicationLogin, ApplicationRole, int, ApplicationClaim, int>();
                _configuration.AddMapping(mappingHelper.GetMappingsToMatchEfIdentity());
                DefaultSessionFactory = _configuration.BuildSessionFactory();
            }
        }
    }
}

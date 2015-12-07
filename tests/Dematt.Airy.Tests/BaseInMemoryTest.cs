using System.Diagnostics;
using System.IO;
using System.Linq;
using Dematt.Airy.Nhibernate.Extensions;
using Dematt.Airy.Tests.Domain;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace Dematt.Airy.Tests
{
    /// <summary>
    /// Base class for in memory tests using SQLite.
    /// </summary>
    [TestFixture]
    public abstract class BaseInMemoryTest
    {
        /// <summary>
        /// The NHibernate session factory we only want one of these :-)
        /// </summary>
        private static ISessionFactory _sessionFactory;

        /// <summary>
        /// Gets or sets the NHibernate session the test.
        /// </summary>
        /// <remarks>We currently only have one of these for the entire test.</remarks>
        protected ISession Session { get; set; }

        /// <summary>
        /// Sets up the test. Creates an NHibernate Session factory and opens a session.
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            CreateNHibernateSessionFactory();
            Session = _sessionFactory.OpenSession();
        }

        /// <summary>
        /// Disposes of the objects once the test has completed.
        /// </summary>
        [TestFixtureTearDown]
        public void TearDown()
        {
            Session.Dispose();
        }

        /// <summary>
        /// Builds the database schema.
        /// </summary>
        /// <param name="config">Configuration containing the database schema that needs to be built.</param>
        public void BuildSchema(Configuration config)
        {
            // Build the schema.
            var createSchemaSql = new StringWriter();
            var schemaExport = new SchemaExport(config);

            // Drop the existing schema.
            schemaExport.Drop(true, true);

            // Print the Sql that will be used to build the schema.
            schemaExport.Create(createSchemaSql, false);
            Debug.Print(createSchemaSql.ToString());

            // Create the schema.
            schemaExport.Create(false, true);
        }

        /// <summary>
        /// Creates the NHibernate session factory.
        /// </summary>
        private void CreateNHibernateSessionFactory()
        {
            if (_sessionFactory == null)
            {
                var configuration = new Configuration();
                configuration.Configure();
                configuration.AddMapping(GetMappingsForSqlite());
                BuildSchema(configuration);
                _sessionFactory = configuration.BuildSessionFactory();

            }
        }

        private HbmMapping GetMappingsForSqlite()
        {
            // Map all classes in Dematt.Airy.Tests.Domain namespace, making sure to exclude any abstract classes.
            var type = typeof(SimpleDomainObject);
            var types = type.Assembly.GetExportedTypes()
                .Where(t => t.Namespace == type.Namespace && t.IsAbstract == false);

            // Create a ModelMapper and apply some custom actions to the events.
            var mapper = new ConventionModelMapper();
            mapper.IsRootEntity(ModelMapperHelper.ApplyAbstractClassesAsRootEntites);
            mapper.BeforeMapProperty += ModelMapperHelper.ApplyDateTimeOffsetSplitTypeToDateTimeOffset;
            return mapper.CompileMappingFor(types);
        }
    }
}

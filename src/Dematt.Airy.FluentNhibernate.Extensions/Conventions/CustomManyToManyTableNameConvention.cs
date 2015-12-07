using System;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Inspections;
using Inflector;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies a custom many to many table name convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class CustomManyToManyTableNameConvention : ManyToManyTableNameConvention
    {
        /// <summary>
        /// Gets the name of a bidirectional table.
        /// </summary>
        /// <param name="collection">The parent <see cref = "IManyToManyCollectionInspector" />.</param>
        /// <param name="otherSide">The child <see cref = "IManyToManyCollectionInspector" />.</param>
        /// <returns>A string containing the name of the bidirectional table.</returns>
        protected override string GetBiDirectionalTableName(IManyToManyCollectionInspector collection, IManyToManyCollectionInspector otherSide)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            if (otherSide == null)
            {
                throw new ArgumentNullException("otherSide");
            }

            return collection.EntityType.Name.Pluralize() + otherSide.EntityType.Name.Pluralize() + "Link";
        }

        /// <summary>
        /// Gets the name of a unidirectional table.
        /// </summary>
        /// <param name="collection">An <see cref = "IManyToManyCollectionInspector" />.</param>
        /// <returns>A string containing the name of the unidirectional table.</returns>
        protected override string GetUniDirectionalTableName(IManyToManyCollectionInspector collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            return collection.EntityType.Name.Pluralize() + collection.ChildType.Name.Pluralize() + "Link";
        }
    }
}
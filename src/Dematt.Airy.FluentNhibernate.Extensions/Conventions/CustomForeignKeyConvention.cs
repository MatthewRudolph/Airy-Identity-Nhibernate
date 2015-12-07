using System;
using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace Dematt.Airy.FluentNhibernate.Extensions.Conventions
{
    /// <summary>
    /// Specifies a custom foreign key convention used by NHibernate for automatically mapping domain entities to a database schema.
    /// </summary>
    public class CustomForeignKeyConvention : ForeignKeyConvention
    {
        /// <summary>
        /// Determines the foreign key name for the specified <see cref = "Type" /> and <see cref = "Member" />.
        /// </summary>
        /// <param name="property">A <see cref = "Member" /></param>
        /// <param name="type">A <see cref = "Type" /></param>
        /// <returns>The foreign key name as a <see cref = "String" /></returns>
        protected override string GetKeyName(Member property, Type type)
        {
            if (property == null)
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }

                return type.Name + "Id";
            }

            return property.Name + "Id";
        }
    }
}
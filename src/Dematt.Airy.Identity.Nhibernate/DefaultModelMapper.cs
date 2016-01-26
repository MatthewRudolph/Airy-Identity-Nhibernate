using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dematt.Airy.Core.Attributes;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace Dematt.Airy.Identity.Nhibernate
{
    public class DefaultModelMapper : ConventionModelMapper
    {
        public DefaultModelMapper()
        {
            DefaltStringLength = SqlClientDriver.MaxSizeForLengthLimitedString + 1;
            DefaltStringIdLength = 128;

            // Set mapper to ignore abstract classes.
            IsRootEntity((type, wasDeclared) => type.IsAbstract == false);

            // Set mapper to use native generator for int Ids, Generators.GuidComb generator for Guid Ids and string length of 128 for string Ids.
            BeforeMapClass += OnMapperOnBeforeMapClass;

            // Set mapper to set a fields size to the properties StringLength attribute if it has one, and non-nullable types to be not nullable in the database.
            BeforeMapProperty += OnMapperOnBeforeMapProperty;
        }

        /// <summary>
        /// Allows the default length for string properties to be changed.
        /// Default value is to use maximum permitted by the database, e.g. VARCHAR(MAX) for Microsoft SQL Server.
        /// </summary>
        public int DefaltStringLength { get; set; }

        /// <summary>
        /// Allows the default length for string properties to be changed.
        /// Default value is to use maximum permitted by the database, e.g. VARCHAR(MAX) for Microsoft SQL Server.
        /// </summary>
        public int DefaltStringIdLength { get; set; }

        /// <summary>
        /// Sets the mapper to use:
        ///  1) a properties StringLength attribute if it has one for the databases field size.
        ///  2) non-nullable types to be not nullable in the database.
        ///  3) creates indexes based on the index attributes of properties.
        /// </summary>
        private void OnMapperOnBeforeMapProperty(IModelInspector inspector, PropertyPath member, IPropertyMapper customizer)
        {
            // Get all the custom attributes.
            var customAttributes = member.LocalMember.GetCustomAttributes(false);

            //TODO: Loop
            // For all types check for index attribute and add index if required.
            IndexAttribute indexAttribute = (IndexAttribute)customAttributes.FirstOrDefault(x => x.GetType() == typeof(IndexAttribute));
            if (indexAttribute != null)
            {
                if (indexAttribute.Unique)
                {
                    customizer.UniqueKey(indexAttribute.Name);
                }
                else
                {
                    customizer.Index(indexAttribute.Name);
                }
            }

            // For string types check for string length attribute and set field length if required
            Type memberType = member.LocalMember.GetPropertyOrFieldType();
            if (memberType == typeof(string))
            {
                StringLengthAttribute stringlengthAttribute = (StringLengthAttribute)customAttributes.FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute));
                int length = DefaltStringLength;
                if (stringlengthAttribute != null && stringlengthAttribute.MaximumLength > 0)
                {
                    length = stringlengthAttribute.MaximumLength;
                }
                customizer.Length(length);
            }

            // For all types if the type is not nullable then set not nullable to true.
            if (!IsNullable(memberType))
            {
                customizer.NotNullable(true);
            }
        }

        /// <summary>
        /// Sets the mapper to use:
        ///  1) a native generator for int primary keys
        ///  2) a Generators.GuidComb generator for Guid primary keys
        ///  3) a string length of 128 for string primary keys
        /// </summary>
        private void OnMapperOnBeforeMapClass(IModelInspector inspector, Type type, IClassAttributesMapper customizer)
        {
            foreach (var p in type.GetProperties())
            {
                if (inspector.IsPersistentId(p))
                {
                    var idType = p.PropertyType;
                    if (idType == typeof(int))
                    {
                        customizer.Id(x => x.Generator(Generators.Native));
                    }
                    else if (idType == typeof(string))
                    {
                        var customAttributes = p.GetCustomAttributes(false);
                        StringLengthAttribute stringlengthAttribute = (StringLengthAttribute)customAttributes.FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute));
                        int length = DefaltStringIdLength;
                        if (stringlengthAttribute != null && stringlengthAttribute.MaximumLength > 0)
                        {
                            length = stringlengthAttribute.MaximumLength;
                        }
                        customizer.Id(x => x.Length(length));
                    }
                    else if (idType == typeof(Guid))
                    {
                        customizer.Id(x => { x.Generator(Generators.GuidComb); });
                    }
                }
            }
        }

        /// <summary>
        /// Returns if a type is nullable or not.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if the type passed is nullable, otherwise false</returns>
        private static bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // Type is a reference type so must be nullable.
            if (Nullable.GetUnderlyingType(type) != null) return true; // Type is a value type of Nullable<T> so must be nullable.
            return false; // Otherwise the type must be a value type, so isn't nullable.
        }
    }
}

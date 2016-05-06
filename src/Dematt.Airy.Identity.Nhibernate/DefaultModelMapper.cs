using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dematt.Airy.Core.Attributes;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace Dematt.Airy.Identity.Nhibernate
{
    public class DefaultModelMapper : ConventionModelMapper
    {
        /// <summary>
        /// The suffix (added to the name of the property) used for the name for foreign key fields.
        /// </summary>
        private readonly string _foreignKeyColumnSuffix = "Id";

        /// <summary>
        /// The prefix (added before the name of the property) used for the name for foreign key.
        /// </summary>
        private readonly string _foreignKeyNamePrefix = "FK_";

        /// <summary>
        /// The string inserted between the names of the two tables being linked in a many to many relationship to create the link table name.
        /// </summary>
        private readonly string _manyToManyLinkTableInsert = "To";

        /// <summary>
        /// Creates a new model mapper with the convention naming style using the default values.
        /// </summary>
        public DefaultModelMapper()
        {
            DeafultMapperSetup();
            AddNamingConventionsToMapper();
        }

        /// <summary>
        /// Creates a new model mapper with or without the convention naming style using the default values.
        /// </summary>
        /// <param name="useConventionMapping">true to use the convention naming style mapping, false to use the mapping by code base style mapping.</param>
        public DefaultModelMapper(bool useConventionMapping)
        {
            DeafultMapperSetup();
            if (useConventionMapping)
            {
                AddNamingConventionsToMapper();
            }
        }

        /// <summary>
        /// Creates a new model mapper with the convention naming style using the supplied values.
        /// </summary>
        /// <param name="foreignKeyColumnSuffix">The suffix to use for foreign key columns.</param>
        /// <param name="manyToManyLinkTableInsert">The insert to use between the referenced table names for a link table name.</param>
        /// <param name="foreignKeyNamePrefix">The prefix to use before the foreign key name.</param>
        public DefaultModelMapper(string foreignKeyColumnSuffix, string manyToManyLinkTableInsert, string foreignKeyNamePrefix)
        {
            _foreignKeyColumnSuffix = foreignKeyColumnSuffix;
            _manyToManyLinkTableInsert = manyToManyLinkTableInsert;
            _foreignKeyNamePrefix = foreignKeyNamePrefix;
            DeafultMapperSetup();
            AddNamingConventionsToMapper();
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
        /// Sets the mapper conventions that are always applied to this mapper.
        /// </summary>
        private void DeafultMapperSetup()
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
        /// Sets the naming conventions that are optional applied to this mapper.
        /// </summary>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void AddNamingConventionsToMapper()
        {
            // Add the foreign key column suffix to the foreign key fields.
            BeforeMapManyToOne += (inspector, member, customizer) =>
            {
                string columnName = member.LocalMember.Name + _foreignKeyColumnSuffix;
                customizer.Column(columnName);
                string tableName = member.LocalMember.DeclaringType.Name;
                string foreignKeyName = string.Format("{0}{1}_{2}", _foreignKeyNamePrefix, tableName, columnName);
                customizer.ForeignKey(foreignKeyName);

            };
            BeforeMapManyToMany += (inspector, member, customizer) =>
            {
                string columnName = member.GetCollectionElementType().Name + _foreignKeyColumnSuffix;
                customizer.Column(columnName);
                string tableName = GetManyToManyLinkTableName(member);
                string foreignKeyName = string.Format("{0}{1}_{2}", _foreignKeyNamePrefix, tableName, columnName);
                customizer.ForeignKey(foreignKeyName);

            };
            BeforeMapJoinedSubclass += (inspector, type, customizer) =>
            {
                customizer.Key(k =>
                {
                    string columnName = type.BaseType.Name + _foreignKeyColumnSuffix;
                    k.Column(columnName);
                    string tableName = type.DeclaringType.Name;
                    string foreignKeyName = string.Format("{0}{1}_{2}", _foreignKeyNamePrefix, tableName, columnName);
                    k.ForeignKey(foreignKeyName);
                });
            };

            // Add Collection mapping conventions.
            BeforeMapSet += BeforeMappingCollectionConvention;
            BeforeMapBag += BeforeMappingCollectionConvention;
            BeforeMapList += BeforeMappingCollectionConvention;
            BeforeMapIdBag += BeforeMappingCollectionConvention;
            BeforeMapMap += BeforeMappingCollectionConvention;
        }

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

            // For all types check for index attributes and add indexes if required.
            var indexAttributes = customAttributes.OfType<IndexAttribute>();
            foreach (var indexAttribute in indexAttributes)
            {
                string indexPrefix = member.GetContainerEntity(inspector).Name;
                if (indexAttribute.Unique)
                {
                    string indexName = string.Format("UI_{0}_{1}", indexPrefix, indexAttribute.Name);
                    customizer.UniqueKey(indexName);
                }
                else
                {
                    string indexName = string.Format("IX_{0}_{1}", indexPrefix, indexAttribute.Name);
                    customizer.Index(indexName);
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
        /// Sets the following conventions:
        /// 1) Foreign key fields are named as the property name suffixed by the value of _foreignKeyColumnSuffix.
        /// 2) Many to Many link tables are named as the object type names sorted alphabetically with the _manyToManyLinkTableInsert inserted inbetween them.
        /// </summary>
        private void BeforeMappingCollectionConvention(IModelInspector inspector, PropertyPath member, ICollectionPropertiesMapper customizer)
        {
            string tableName;
            if (inspector.IsManyToMany(member.LocalMember))
            {
                tableName = GetManyToManyLinkTableName(member);
                customizer.Table(tableName);
            }
            else
            {
                tableName = member.GetCollectionElementType().Name;
            }

            string columnName = GetKeyColumnName(inspector, member);
            string foreignKeyName = string.Format("{0}{1}_{2}", _foreignKeyNamePrefix, tableName, columnName);
            customizer.Key(k =>
            {
                k.Column(columnName);
                k.ForeignKey(foreignKeyName);
            });
        }

        /// <summary>
        /// Gets the Many to Many link tables name.
        /// </summary>
        private string GetManyToManyLinkTableName(PropertyPath member)
        {
            return String.Join(_manyToManyLinkTableInsert, GetManyToManySidesNames(member).OrderBy(x => x));
        }

        /// <summary>
        /// Gets the object type names for a many to many relationship sorted alphabetically.
        /// </summary>
        private static IEnumerable<string> GetManyToManySidesNames(PropertyPath member)
        {
            yield return member.GetRootMemberType().Name;
            yield return member.GetCollectionElementType().Name;
        }

        /// <summary>
        /// Gets the foreign key field name to use for a property.
        /// </summary>
        private string GetKeyColumnName(IModelInspector inspector, PropertyPath member)
        {
            var otherSideProperty = member.OneToManyOtherSideProperty();
            if (inspector.IsOneToMany(member.LocalMember) && otherSideProperty != null)
            {
                return otherSideProperty.Name + _foreignKeyColumnSuffix;

            }

            return member.GetRootMemberType().Name + _foreignKeyColumnSuffix;
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

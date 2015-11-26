using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Dematt.Airy.Identity.Nhibernate.Contracts;
using Microsoft.AspNet.Identity;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Driver;
using NHibernate.Mapping.ByCode;

namespace Dematt.Airy.Identity.Nhibernate
{
    /// <summary>
    /// Helper class to create NHibernate Mappings for the ASP.Net Identity Entities.
    /// </summary>
    /// <typeparam name="TUser">The User type to map.</typeparam>
    /// <typeparam name="TUserKey">The type used for the User types unique key.</typeparam>
    /// <typeparam name="TLogin">The Login type to map.</typeparam>
    /// <typeparam name="TRole">The Role type to map.</typeparam>
    /// <typeparam name="TRoleKey">The type used for the Role types unique key.</typeparam>
    /// <typeparam name="TClaim">The Claim type to map.</typeparam>
    /// <typeparam name="TClaimKey">The type used for the Claim types unique key.</typeparam>
    public class MappingHelper<TUser, TUserKey, TLogin, TRole, TRoleKey, TClaim, TClaimKey>
        where TUser : class, IIdentityUser<TUserKey, TLogin, TRole, TRoleKey, TClaim>
        where TLogin : class, IIdentityUserLogin<TUser>
        where TClaim : class, IIdentityUserClaim<TUser, TClaimKey>
        where TRole : class, IIdentityRole<TUser, TRoleKey>, IRole<TRoleKey>
    {
        /// <summary>
        /// Static constructor, called automatically to initialize the class before the first instance is created or any static members are referenced. 
        /// </summary>
        public MappingHelper()
        {
            DefaltStringLength = SqlClientDriver.MaxSizeForLengthLimitedString + 1;
        }

        /// <summary>
        /// The name to use for the Many to Many link table for the User to Roles relationship.
        /// </summary>
        private const string UsersRolesLinkTableName = "AspNetUserRoles";

        /// <summary>
        /// The name to use for the for the User Id foreign keys.
        /// </summary>
        private const string UserIdForeignKeyFieldName = "UserId";

        /// <summary>
        /// The name to use for the for the Role Id foreign keys.
        /// </summary>
        private const string RoleIdForeignKeyFieldName = "RoleId";

        /// <summary>
        /// Allows the default length for string properties to be changed.
        /// Default value is to use maximum permitted by the database, e.g. VARCHAR(MAX) for Microsoft SQL Server.
        /// </summary>
        public int DefaltStringLength { get; set; }

        public HbmMapping GetMappingsToMatchEfIdentity()
        {
            // TODO:  Test for when the types have not abstract base classes.  We 'll probably need to exclude them form the mappings.
            //var baseEntityToIgnore = new[] {
            //    typeof(TUser).BaseType,
            //    typeof(TLogin).BaseType,
            //    typeof(TRole).BaseType,
            //    typeof(TClaim).BaseType
            //};

            //Could check types here and if not default then add to ignore?  But would be best if we could find out way to ignore classes that have been inherited.
            var entitiesToMap = new List<Type>
            {
                typeof(TUser),
                typeof(TLogin),
                typeof(TRole),
                typeof(TClaim)
            };

            // Set up the Model Mapper and and provide it with some customisations so it matches the EF style...
            var mapper = new ConventionModelMapper();

            // Set mapper to ignore abstract classes
            Func<Type, bool, bool> matchRootEntity = (type, wasDeclared) => type.IsAbstract == false;
            mapper.IsRootEntity(matchRootEntity);

            // Set mapper to use native generator for int Ids and string length of 128 for string Ids.
            mapper.BeforeMapClass += (inspector, type, customizer) =>
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
                            customizer.Id(x => x.Length(128));
                        }
                        else if (idType == typeof(Guid))
                        {
                            customizer.Id(x =>
                            {
                                x.Generator(Generators.GuidComb);
                            });
                        }
                    }
                }
            };
            // Set mapper to use the a properties StringLength attribute if it has one, and non-nullable types to be not nullable in the database.
            mapper.BeforeMapProperty += (inspector, member, customizer) =>
            {
                Type memberType = member.LocalMember.GetPropertyOrFieldType();
                if (memberType == typeof(string))
                {
                    var customAttributes = member.LocalMember.GetCustomAttributes(false);
                    StringLengthAttribute stringlengthAttribute = (StringLengthAttribute)customAttributes.FirstOrDefault(x => x.GetType() == typeof(StringLengthAttribute));
                    int length = DefaltStringLength;
                    if (stringlengthAttribute != null && stringlengthAttribute.MaximumLength > 0)
                    {
                        length = stringlengthAttribute.MaximumLength;
                    }
                    customizer.Length(length);
                }

                if (!IsNullable(memberType))
                {
                    customizer.NotNullable(true);
                }
            };

            // Map the generic User class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            mapper.Class<TUser>(c =>
            {
                c.Table("AspNetUsers");
                c.Property(
                    x => x.UserName,
                    m =>
                    {
                        m.Length(256);
                        m.NotNullable(true);
                        m.UniqueKey("UX_Users_UserName");
                    });
                c.Property(
                    x => x.Email,
                    m =>
                    {
                        m.Length(256);
                    });
                c.Set(
                    x => x.Logins,
                    m =>
                    {
                        m.Key(s =>
                        {
                            s.Column(UserIdForeignKeyFieldName);
                            s.OnDelete(OnDeleteAction.Cascade);
                            s.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId");
                        });
                        m.Inverse(true);
                        m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    });
                c.Set(
                    x => x.Roles,
                    m =>
                    {
                        m.Table(UsersRolesLinkTableName);
                        m.Key(k =>
                        {
                            k.Column(f =>
                            {
                                f.Name(UserIdForeignKeyFieldName);
                                f.NotNullable(true);
                                f.Index("IX_UserRoles_UserId");
                            });
                        });
                        m.Inverse(false);
                    },
                    r =>
                    {
                        r.ManyToMany(p =>
                        {
                            p.Column(RoleIdForeignKeyFieldName);
                            p.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId");
                        });
                    });
                c.Bag(
                    x => x.Claims,
                    m =>
                    {
                        m.Key(s =>
                        {
                            s.Column(UserIdForeignKeyFieldName);
                            s.OnDelete(OnDeleteAction.Cascade);
                            s.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId");
                        });
                        m.Inverse(true);
                        m.Cascade(Cascade.All | Cascade.DeleteOrphans);
                    },
                    r =>
                    {
                        r.OneToMany();
                    });
            });

            // Map the generic Login class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            mapper.Class<TLogin>(c =>
            {
                c.Table("AspNetUserLogins");
                c.ComposedId(k =>
                {
                    k.Property(
                        x => x.LoginProvider,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.Property(
                        x => x.ProviderKey,
                        m =>
                        {
                            m.Column(f => f.Length(128));
                        });
                    k.ManyToOne(x => x.User, m =>
                    {
                        m.Column(UserIdForeignKeyFieldName);
                        m.Index("IX_Logins_UserId");
                    });
                });
            });

            // Map the generic Role class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            mapper.Class<TRole>(c =>
            {
                c.Table("AspNetRoles");
                c.Property(
                    x => x.Name,
                    m =>
                    {
                        m.Column(f =>
                        {
                            f.Length(256);
                            f.NotNullable(true);
                            f.Unique(true);
                        });
                    });
                c.Set(
                    x => x.Users,
                    m =>
                    {
                        m.Table(UsersRolesLinkTableName);
                        m.Key(k =>
                        {
                            k.Column(f =>
                            {
                                f.Name(RoleIdForeignKeyFieldName);
                                f.NotNullable(true);
                                f.Index("IX_UserRoles_RoleId");
                            });
                        });
                        m.Inverse(true);
                    },
                    r =>
                    {
                        r.ManyToMany(p =>
                        {
                            p.Column(UserIdForeignKeyFieldName);
                            p.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId");
                        });
                    });
            });

            // Map the generic Claim class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            mapper.Class<TClaim>(c =>
            {
                c.Table("AspNetUserClaims");
                c.ManyToOne(
                    x => x.User,
                    m =>
                    {
                        m.Column(f =>
                        {
                            f.Name(UserIdForeignKeyFieldName);
                            f.NotNullable(true);
                            f.Index("IX_UserClaims_UserId");
                        });
                    });
                c.Property(
                    x => x.ClaimType,
                    m =>
                    {
                        m.Column(f => f.Length(SqlClientDriver.MaxSizeForLengthLimitedString + 1));
                    });
                c.Property(
                    x => x.ClaimValue,
                    m =>
                    {
                        m.Column(f => f.Length(SqlClientDriver.MaxSizeForLengthLimitedString + 1));
                    });
            });

            return mapper.CompileMappingFor(entitiesToMap);
        }

        private static bool IsNullable(Type type)
        {
            if (!type.IsValueType) return true; // Type is a reference type so must be nullable.
            if (Nullable.GetUnderlyingType(type) != null) return true; // Type is a value type of Nullable<T> so must be nullable.
            return false; // Otherwise the type must be a value type, so isn't nullable.
        }
    }
}

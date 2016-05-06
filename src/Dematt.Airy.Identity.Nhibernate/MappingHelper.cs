using System;
using System.Collections.Generic;
using Dematt.Airy.Identity.Contracts;
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
        /// Default constructor. 
        /// </summary>
        public MappingHelper()
        {
            // The default model mapper class:
            //  ** Ignores abstract classes.
            //  ** Uses native generator for int Ids, Generators.GuidComb generator for Guid Ids and string length of 128 for string Ids.
            //  ** Sets a fields size to the properties StringLength attribute if it has one.
            //  ** Sets non-nullable types to be not nullable in the database.
            //  ** Creates indexes based on the index attributes of properties.
            Mapper = new DefaultModelMapper();
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
        /// The model mapper contains the conventions used for mappings.
        /// </summary>
        public DefaultModelMapper Mapper { get; private set; }

        public HbmMapping GetMappingsToMatchEfIdentity()
        {
            //Get the types to map.
            var entitiesToMap = new List<Type>
            {
                typeof(TUser),
                typeof(TLogin),
                typeof(TRole),
                typeof(TClaim)
            };

            // Map the generic User class to match the EF style.
            // c=>class, m=>map, x=>entity, k=>key, f=>field
            Mapper.Class<TUser>(c =>
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
                            k.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId");
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
            Mapper.Class<TLogin>(c =>
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
            Mapper.Class<TRole>(c =>
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
                            k.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId");
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
            Mapper.Class<TClaim>(c =>
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

            return Mapper.CompileMappingFor(entitiesToMap);
        }
    }
}

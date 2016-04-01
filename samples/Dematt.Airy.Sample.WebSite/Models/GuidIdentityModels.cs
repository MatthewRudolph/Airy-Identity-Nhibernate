using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dematt.Airy.Identity;
using Dematt.Airy.Identity.Nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;

namespace Dematt.Airy.Sample.WebSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class GuidApplicationUser : IdentityUser<Guid, GuidApplicationLogin, GuidApplicationRole, string, GuidApplicationClaim>
    {
        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<GuidApplicationUser, Guid> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class GuidApplicationRole : IdentityRole<GuidApplicationUser, string>
    {
        public GuidApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public GuidApplicationRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    public class GuidApplicationLogin : IdentityUserLogin<GuidApplicationUser>
    {

    }

    public class GuidApplicationClaim : IdentityUserClaim<GuidApplicationUser, int>
    {

    }

    public class GuidApplicationUserStore<TUser> : UserStore<GuidApplicationUser, Guid, GuidApplicationLogin, GuidApplicationRole, string, GuidApplicationClaim, int>,
        IUserStore<GuidApplicationUser, Guid>
        where TUser : GuidApplicationUser
    {
        public GuidApplicationUserStore(ISession context)
            : base(context)
        {
        }
    }

    public class GuidApplicationRoleStore<TRole> : RoleStore<GuidApplicationRole, string, GuidApplicationUser>
        where TRole : GuidApplicationRole, new()
    {
        public GuidApplicationRoleStore(ISession context)
            : base(context)
        {
        }
    }
}
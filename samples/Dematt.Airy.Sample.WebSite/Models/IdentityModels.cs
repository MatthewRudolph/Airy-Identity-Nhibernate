using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Dematt.Airy.Identity.Nhibernate;
using Microsoft.AspNet.Identity;
using NHibernate;

namespace Dematt.Airy.Sample.WebSite.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<string, ApplicationLogin, ApplicationRole, string, ApplicationClaim>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationRole : IdentityRole<ApplicationUser, string>
    {
        public ApplicationRole()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    public class ApplicationLogin : IdentityUserLogin<ApplicationUser>
    {

    }

    public class ApplicationClaim : IdentityUserClaim<ApplicationUser, int>
    {

    }

    public class ApplicationUserStore<TUser> : UserStore<ApplicationUser, string, ApplicationLogin, ApplicationRole, string, ApplicationClaim, int>,
        IUserStore<ApplicationUser>
        where TUser : ApplicationUser
    {
        public ApplicationUserStore(ISession context)
            : base(context)
        {
        }
    }

    public class ApplicationRoleStore<TRole> : RoleStore<ApplicationRole, string, ApplicationUser>
        where TRole : ApplicationRole, new()
    {
        public ApplicationRoleStore(ISession context)
            : base(context)
        {
        }
    }
}
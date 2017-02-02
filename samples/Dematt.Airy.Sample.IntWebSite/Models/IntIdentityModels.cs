// REMOVE: The EntityFramework and related using that are no longer required.
//using System.Data.Entity;

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Identity.EntityFramework

// Add using for Dematt.Airy.Identity;
using Dematt.Airy.Identity;
using Dematt.Airy.Identity.Nhibernate;
using NHibernate;

namespace Dematt.Airy.Sample.IntWebSite.Models
{
    // These User, Role, Login and Claim classes match the default schema from the Entity Framework Implementation.

    /// <summary>
    /// The user class.
    /// </summary>
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, ApplicationLogin, ApplicationRole, int, ApplicationClaim>
    {
        public ApplicationUser()
        {
        }

        /// <summary>
        /// This is only required if you are using the Visual Studio ASP.Net MVC template.
        /// If you are using a dependency injection framework then you can simply inject the user manager class where it is needed instead.
        /// </summary>
        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    /// <summary>
    /// The role class.
    /// </summary>
    public class ApplicationRole : IdentityRole<ApplicationUser, int>
    {
        public ApplicationRole()
        {
        }

        public ApplicationRole(string roleName)
            : this()
        {
            Name = roleName;
        }
    }

    /// <summary>
    /// The login class.
    /// </summary>
    public class ApplicationLogin : IdentityUserLogin<ApplicationUser>
    {

    }

    /// <summary>
    /// The claim class.
    /// </summary>
    public class ApplicationClaim : IdentityUserClaim<ApplicationUser, int>
    {

    }

    /// <summary>
    /// The user store class.
    /// </summary>
    public class ApplicationUserStore<TUser> : UserStore<ApplicationUser, int, ApplicationLogin, ApplicationRole, int, ApplicationClaim, int>,
        IUserStore<ApplicationUser, int>
        where TUser : ApplicationUser
    {
        public ApplicationUserStore(ISession context)
            : base(context)
        {
        }
    }

    /// <summary>
    /// The role store class.
    /// </summary>
    public class ApplicationRoleStore<TRole> : RoleStore<ApplicationRole, int, ApplicationUser>
        where TRole : ApplicationRole, new()
    {
        public ApplicationRoleStore(ISession context)
            : base(context)
        {
        }
    }

    // REMOVE: The ApplicationDbContextClass.
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection", throwIfV1Schema: false)
    //    {
    //    }

    //    public static ApplicationDbContext Create()
    //    {
    //        return new ApplicationDbContext();
    //    }
    //}
}
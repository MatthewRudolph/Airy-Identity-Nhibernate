# Airy.Identity.Nhibernate #

[![Build status](https://ci.appveyor.com/api/projects/status/xt2v610omjxosig6/branch/master?svg=true&passingText=master%20-%20passing&failingText=master%20-%20failing&pendingText=master%20-%20pending)](https://ci.appveyor.com/project/MatthewRudolph/airy/branch/master)
[![Build status](https://ci.appveyor.com/api/projects/status/xt2v610omjxosig6/branch/dev?svg=true&passingText=dev%20-%20passing&failingText=dev%20-%20failing&pendingText=dev%20-%20pending)](https://ci.appveyor.com/project/MatthewRudolph/airy/branch/dev)

## About ##
NHibenrate implementation of the provider for ASP.Net Identity that can easily be used to replace the default Entity Framework provider.

## Features ##
* Drop-in replacement for ASP.Net Identity Entity Framework (v2.2.1) backing store that ships with ASP.Net MVC 5.
* By default matches with database design used by the Microsoft.AspNet.Identity.EntityFramework provider.
* Supports the use of types other than strings for primary keys such as Guids or integers.
* Supports additional properties on the User, Role, Login and Claim classes if required.
* Fully implements all of the UserStore and RoleStore interfaces.
* Sample ASP.Net MVC 5 project.

## Getting Started ##
If you have the Entity Framework version of ASP.Net Identity then uninstall it using nuget.
```Powershell
Uninstall-Package Microsoft.AspNet.Identity.EntityFramework
```
You can also uninstall Entity Framework itself unless you need it for some reason.
```Powershell
Uninstall-Package EntityFramework
```
Install the Airy Identity Nhibernate package.
```Powershell
Install-Package Dematt.Airy.Identity.Nhibernate
```



The library allow you to use you own primary key types for users, roles and claims, this means that you need to create those classes.
The base classes in the Dematt.Airy.Identity namespace have all the required functionality your classes just need to inherit from them and provide the key types.

A set of classes to match the default classes and schema of the Microsoft.AspNet.Identity.EntityFramework implementation would look like this:
```C#
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
    /// <summary>
    /// The user class.
    /// </summary>
    public class ApplicationUser : IdentityUser<string, ApplicationLogin, ApplicationRole, string, ApplicationClaim>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// This is only required if you are using the Visual Studio ASP.Net MVC template.
        /// If you are using a dependency injection framework then you can simply inject the user manager class where it is needed instead.
        /// </summary>
        public virtual async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
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
    public class ApplicationUserStore<TUser> : UserStore<ApplicationUser, string, ApplicationLogin, ApplicationRole, string, ApplicationClaim, int>,
        IUserStore<ApplicationUser>
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
    public class ApplicationRoleStore<TRole> : RoleStore<ApplicationRole, string, ApplicationUser>
        where TRole : ApplicationRole, new()
    {
        public ApplicationRoleStore(ISession context)
            : base(context)
        {
        }
    }
}
```

This would use the default primary key type of string for the User and Role entities and int for the claim entity, the string primary keys will be populated with a random Guid, the int one will be a assigned by the database.
This matches the default configuration and schema of the Entity Framework version, and should be compatible with existing databases created using the Entity Framework version.

If you require a different primary key type for User, Role and Claim then you can replace the keys as appropriate.  
Example classes using Guid for all primary keys can be found here:  
Example classes using int for all primary keys can be found here:  
This article https://www.asp.net/identity/overview/extensibility/change-primary-key-for-users-in-aspnet-identity details the other changes that will be necessary to your configuration if you change the Id type of the Identity entities.
There are sample websites for both all Guid and all int primary keys. They are a default (VS2013) MVC web application project with the minimum changes made to support each of the primary key types.


## Acknowledgements ##
There is already an excellent official [NHibernate Implementation] (https://github.com/nhibernate/NHibernate.AspNet.Identity) of the ASP.Net Identity.
However it does not currently allow the use of types other than strings for primary keys [Issue 46] (https://github.com/nhibernate/NHibernate.AspNet.Identity/issues/46)  
As I needed to be able to use Guids for some primary keys I forked it and started to see how I could implement it.

It was a lot harder than I thought especially since I'm not an NHibernate Guru.  I found that I needed to take a different approach to the current implementation and decided it would be easier to do if I started from scratch with a new solution.
I leant heavily on both the existing [official NHibernate Implementation] (https://github.com/nhibernate/NHibernate.AspNet.Identity) and also the [Microsoft Entity Framework Implementation] (https://aspnetidentity.codeplex.com/)  
So a huge thank you to those authors particularly https://github.com/milesibastos for their work and for open sourcing it, without them I would never had been able to complete this, my first open source project.

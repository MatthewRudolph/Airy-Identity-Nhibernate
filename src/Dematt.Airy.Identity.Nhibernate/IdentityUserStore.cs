using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNet.Identity;
using NHibernate;

namespace Dematt.Airy.Identity.Nhibernate
{
    [SuppressMessage("ReSharper", "UnusedTypeParameter")]
    public class IdentityUserStore<TUser> :
        UserStore<IdentityUser, string, IdentityUserLogin, IdentityRole, string, IdentityUserClaim, int>,
        IUserStore<IdentityUser>
        where TUser : IdentityUser
    {

        public IdentityUserStore(ISession context)
            : base(context)
        {
        }
    }
}

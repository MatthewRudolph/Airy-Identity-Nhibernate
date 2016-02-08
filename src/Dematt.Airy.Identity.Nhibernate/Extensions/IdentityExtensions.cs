using System;
using System.Security.Principal;
using Microsoft.AspNet.Identity;

namespace Dematt.Airy.Identity.Nhibernate.Extensions
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// <![CDATA[The default ASP.Net Identity extension method User.Identity.GetUserId<>() will not work for Guids as they don't implement IConvertible.]]>
        /// <![CDATA[This extension method will attempt to parse out the string Id value into a guid, if it can't parse it will return Guid.Empty.]]>
        /// </summary>
        public static Guid GetGuidUserId(this IIdentity identity)
        {
            Guid result;
            Guid.TryParse(identity.GetUserId(), out result);
            return result;
        }
    }
}

using System;
using System.Reflection;
using NHibernate.Mapping.ByCode;

namespace Dematt.Airy.Identity.Nhibernate
{
    public static class PropertyPathExtensions
    {
        public static Type GetRootMemberType(this PropertyPath member)
        {
            return member.GetRootMember().DeclaringType;
        }

        public static Type GetCollectionElementType(this PropertyPath member)
        {
            return member.LocalMember.GetPropertyOrFieldType().DetermineCollectionElementOrDictionaryValueType();
        }

        public static MemberInfo OneToManyOtherSideProperty(this PropertyPath member)
        {
            return member.GetCollectionElementType().GetFirstPropertyOfType(member.GetRootMemberType());
        }
    }
}

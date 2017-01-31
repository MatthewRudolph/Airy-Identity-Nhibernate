using System.Web;
using System.Web.Mvc;

namespace Dematt.Airy.Sample.IntWebSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

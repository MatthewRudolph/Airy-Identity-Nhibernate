using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dematt.Airy.Sample.WebSite.Startup))]
namespace Dematt.Airy.Sample.WebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Dematt.Airy.Sample.IntWebSite.Startup))]
namespace Dematt.Airy.Sample.IntWebSite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

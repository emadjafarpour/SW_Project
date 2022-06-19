using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BitakShop.Web.Startup))]
namespace BitakShop.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

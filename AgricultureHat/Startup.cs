using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AgricultureHat.Startup))]
namespace AgricultureHat
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PureSmileUI.Startup))]
namespace PureSmileUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

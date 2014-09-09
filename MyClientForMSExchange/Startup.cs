using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyClientForMSExchange.Startup))]
namespace MyClientForMSExchange
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestSignalR.Startup))]
namespace TestSignalR
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Ineventation.WebApp;
using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(Ineventation.WebApp.Startup))]
namespace Ineventation.WebApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
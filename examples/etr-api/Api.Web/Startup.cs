using Castle.Windsor;
using Microsoft.Owin;
using Owin;
using Api.Web.Windsor;

[assembly: OwinStartupAttribute("PublicOwinConfig", typeof(Api.Web.Startup))]
namespace Api.Web
{
    public class Startup
    {
        private static IWindsorContainer container;
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            container = Bootstrapper.InitializeContainer();
        }
    }
}

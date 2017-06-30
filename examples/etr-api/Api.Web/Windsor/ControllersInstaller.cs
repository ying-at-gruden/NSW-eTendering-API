using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.Owin.Security;

namespace Api.Web.Windsor
{
    public class ControllersInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Windsor installer for this project
        /// Important: When adding services always include the Lifestyle. This will help in identifying issues 
        /// related to transient instances being held captive within singletons.
        /// </summary>
        /// <param name="container">passed in from framework</param>
        /// <param name="store">passed in from framework</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                .BasedOn<IController>()
                .LifestyleTransient());

            container.Register(Classes.FromThisAssembly()
                .BasedOn<IHttpController>()
                .LifestyleTransient());
        }
    }
}
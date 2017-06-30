using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Api.Utilities.Windsor;

namespace Api.Web.Windsor
{
    public static class Bootstrapper
    {
        private static bool _initialised = false;

        public static IWindsorContainer InitializeContainer()
        {
            var container = IocHelper.Container;

            if (_initialised) return container;

            container.Register(
                Component.For<IWindsorContainer>().Instance(container)
                );

            container.Install(
                    FromAssembly.Containing<UtilitiesInstaller>(),
                    FromAssembly.Containing<ControllersInstaller>()
                );

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorHttpControllerActivator(container));

            _initialised = true;

            return container;
        }


        public static IWindsorContainer InitializeContainer(ConfigurationInstaller config)
        {
            // Windsor configuration
            var container = IocHelper.Container;

            if (_initialised) return container;

            container.Register(
                    Component.For<IWindsorContainer>().Instance(container)
                );

            container.Install(
                    config,
                    FromAssembly.InThisApplication()
                );

            var controllerFactory = new WindsorControllerFactory(container.Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator),
                new WindsorHttpControllerActivator(container));

            _initialised = true;

            return container;
        }

        public static void Release()
        {
            IocHelper.Release();

            _initialised = false;
        }
    }
}
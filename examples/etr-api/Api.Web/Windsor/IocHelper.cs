using Castle.Windsor;
using Castle.Windsor.Installer;
using Component = Castle.MicroKernel.Registration.Component;

namespace Api.Web.Windsor
{
    public static class IocHelper
    {

        private static IWindsorContainer _container;

        public static IWindsorContainer Container 
            => _container ?? (_container = new WindsorContainer());

        public static void Release()
        {
            _container.Dispose();

            _container = null;
        }

    }
}

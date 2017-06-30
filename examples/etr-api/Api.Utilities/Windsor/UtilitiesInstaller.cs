using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Api.Utilities.Aws;
using Api.Utilities.FileStorage;
using Api.Utilities.ViewHelpers;

namespace Api.Utilities.Windsor
{
    public class UtilitiesInstaller : IWindsorInstaller
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
            container.Register(
                Component
                    .For<AwsStorageHelper>()
                    .DependsOn(new
                    {
                        serviceUrl = ConfigurationManager.AppSettings["aws:s3:base_url"]
                    })
                    .LifestyleSingleton()
                );

            container.Register(
                Component
                    .For<AwsStorageClient>()
                    .DependsOn(new
                    {
                        bucketName = ""
                    })
                    .LifestyleTransient()
                );
            
            container.Register(
                Component
                    .For<FormatHelper>()
                    .LifestyleTransient()
                );

            container.Register(
                Component
                    .For<ConfigurationHelper>()
                    .LifestyleTransient()
                );
        }
    }
}
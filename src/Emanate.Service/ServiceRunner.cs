using System.ServiceProcess;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;

namespace Emanate.Service
{
    class ServiceRunner
    {
        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();
            var loader = new ModuleLoader();
            loader.LoadModules(builder);

            builder.RegisterType<EmanateService>();
            builder.RegisterType<EmanateConsole>();
            builder.RegisterType<AppConfigStorage>().As<IConfigurationStorage>();
            builder.RegisterType<ReflectionConfigurationGenerator>().As<IConfigurationGenerator>();

            return builder.Build();
        }

        public void RunAsService()
        {
            var container = CreateContainer();
            var service = container.Resolve<EmanateService>();

            var servicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(servicesToRun);
        }

        public void RunAsConsole()
        {
            var container = CreateContainer();
            var app = container.Resolve<EmanateConsole>();

            app.Start();
        }
    }
}
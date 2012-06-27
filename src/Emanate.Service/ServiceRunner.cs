using System.ServiceProcess;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
using Emanate.Core.Output;
using Emanate.Core.Output.DelcomVdi;

namespace Emanate.Service
{
    class ServiceRunner
    {
        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<EmanateService>();
            builder.RegisterType<EmanateConsole>();
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityMonitor>().As<IBuildMonitor>();
            builder.RegisterType<AppConfigStorage>().As<IConfigurationStorage>();
            builder.RegisterType<ReflectionConfigurationGenerator>().As<IConfigurationGenerator>();
            builder.RegisterType<DelcomOutput>().As<IOutput>();
            builder.Register(c => c.Resolve<IConfigurationGenerator>().Generate<TeamCityConfiguration>()).SingleInstance();

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
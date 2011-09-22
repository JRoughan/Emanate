using System.ServiceProcess;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;
using Emanate.Core.Output;
using Emanate.Core.Output.DelcomVdi;

namespace Emanate.Service
{
    static class Program
    {
        static void Main()
        {
            var serviceRunner = new ServiceRunner();
            serviceRunner.Run();
        }
    }

    class ServiceRunner
    {
        private IContainer CreateContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MonitoringService>();
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityMonitor>().As<IBuildMonitor>();
            builder.RegisterType<AppConfigStorage>().As<IConfigurationStorage>();
            builder.RegisterType<ReflectionConfigurationGenerator>().As<IConfigurationGenerator>();
            builder.RegisterType<DelcomOutput>().As<IOutput>();
            builder.Register(c => c.Resolve<IConfigurationGenerator>().Generate<TeamCityConfiguration>()).SingleInstance();

            return builder.Build();
        }

        public void Run()
        {
            var container = CreateContainer();
            var service = container.Resolve<MonitoringService>();

            var servicesToRun = new ServiceBase[] { service };
            ServiceBase.Run(servicesToRun);
        }
    }
}

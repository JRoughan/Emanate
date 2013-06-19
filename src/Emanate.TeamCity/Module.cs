using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Core.Input.TeamCity;

namespace Emanate.TeamCity
{
    public class Module : IModule
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityMonitor>().As<IBuildMonitor>();
            builder.Register(c => c.Resolve<IConfigurationGenerator>().Generate<TeamCityConfiguration>()).SingleInstance();
        }
    }
}

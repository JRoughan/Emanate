using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Serilog;

namespace Emanate.TeamCity
{
    public class TeamCityModule : IEmanateModule, IModule
    {
        public string Key { get; } = "teamcity2017";
        public string Name { get; } = "TeamCity (2017)";
        public Direction Direction { get; } = Direction.Input;

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Log.Information("=> TeamCityModule.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<TeamCityMonitorFactory>().Keyed<IBuildMonitorFactory>(Key).SingleInstance();
            builder.RegisterType<TeamCityMonitor>();
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> TeamCityModule.RegisterCommon");
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityConfiguration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

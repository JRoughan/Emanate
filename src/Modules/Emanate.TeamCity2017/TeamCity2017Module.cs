using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Serilog;

namespace Emanate.TeamCity2017
{
    public class TeamCity2017Module : IEmanateModule, IModule
    {
        public string Key { get; } = "teamcity2017";
        public string Name { get; } = "TeamCity (2017)";
        public Direction Direction { get; } = Direction.Input;

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Log.Information("=> TeamCity2017Module.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<TeamCity2017MonitorFactory>().Keyed<IBuildMonitorFactory>(Key).SingleInstance();
            builder.RegisterType<TeamCity2017Monitor>();
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> TeamCity2017Module.RegisterCommon");
            builder.RegisterType<TeamCity2017Connection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCity2017Configuration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Extensibility;
using Emanate.TeamCity2017.Admin.Devices;
using Emanate.TeamCity2017.Admin.Inputs;
using Emanate.TeamCity2017.Admin.Profiles;
using Serilog;

namespace Emanate.TeamCity2017.Admin
{
    public class TeamCityAdminModule : IEmanateAdminModule, IModule
    {
        public string Key { get; } = "teamcity2017";
        public string Name { get; } = "TeamCity (2017)";
        public Direction Direction { get; } = Direction.Input;

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> TeamCityModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<TeamCityInputSelectorView>().Keyed<InputSelector>(Key);
            builder.RegisterType<TeamCityInputSelectorViewModel>();

            builder.RegisterType<TeamCityProfileManagerView>().Keyed<ProfileManager>(Key);
            builder.RegisterType<TeamCityDeviceManagerView>().Keyed<DeviceManager>(Key);
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> TeamCityModule.RegisterCommon");
            builder.RegisterType<TeamCity2017Connection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCity2017Configuration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

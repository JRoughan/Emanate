using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Extensibility;
using Emanate.TeamCity.Admin.Devices;
using Emanate.TeamCity.Admin.Inputs;
using Emanate.TeamCity.Admin.Profiles;
using Serilog;

namespace Emanate.TeamCity.Admin
{
    public class TeamCityAdminModule : IEmanateAdminModule, IModule
    {
        public string Key { get; } = "teamcity";
        public string Name { get; } = "TeamCity";
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

        public IConfiguration GenerateDefaultConfig()
        {
            return new TeamCityConfiguration();
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> TeamCityModule.RegisterCommon");
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityConfiguration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Extensibility;
using Emanate.TeamCity.Admin.Configuration;
using Emanate.TeamCity.Admin.Devices;
using Emanate.TeamCity.Admin.InputSelector;
using Serilog;
using TeamCityDeviceManagerView = Emanate.TeamCity.Admin.Devices.TeamCityDeviceManagerView;

namespace Emanate.TeamCity.Admin
{
    public class TeamCityAdminModule : IEmanateAdminModule, IInputModule
    {
        public string Key { get; } = "teamcity";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> TeamCityModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Extensibility.InputSelector>(Key);
            builder.RegisterType<InputSelectorViewModel>();

            builder.RegisterType<TeamCityConfigurationView>().Keyed<ConfigurationEditor>(Key);

            builder.RegisterType<TeamCityDeviceManagerView>().Keyed<DeviceManager>(Key);
            builder.RegisterType<TeamCityDeviceManagerViewModel>();
        }

        public IInputConfiguration GenerateDefaultConfig()
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

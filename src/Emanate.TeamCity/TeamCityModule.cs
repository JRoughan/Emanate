using System.Diagnostics;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Extensibility;
using Emanate.TeamCity.Configuration;
using Emanate.TeamCity.InputSelector;

namespace Emanate.TeamCity
{
    public class TeamCityModule : IInputModule
    {
        public string Key { get; } = "teamcity";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> TeamCityModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Extensibility.InputSelector>(Key);
            builder.RegisterType<InputSelectorViewModel>();

            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(Key);

            builder.RegisterType<TeamCityDeviceManagerView>().Keyed<DeviceManager>(Key);
            builder.RegisterType<TeamCityDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> TeamCityModule.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<TeamCityMonitor>().Keyed<IBuildMonitor>(Key);
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> TeamCityModule.RegisterCommon");
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityConfiguration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

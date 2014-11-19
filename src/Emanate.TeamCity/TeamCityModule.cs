using System.Diagnostics;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Service.Admin;
using Emanate.TeamCity.Configuration;
using Emanate.TeamCity.InputSelector;

namespace Emanate.TeamCity
{
    public class TeamCityModule : IEmanateModule
    {
        private const string key = "teamcity";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> TeamCityModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Service.Admin.InputSelector>(key);
            builder.RegisterType<InputSelectorViewModel>();

            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(key);

            builder.RegisterType<TeamCityDeviceManagerView>().Keyed<DeviceManager>(key);
            builder.RegisterType<TeamCityDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> TeamCityModule.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<TeamCityMonitor>().Keyed<IBuildMonitor>(key);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> TeamCityModule.RegisterCommon");
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityConfiguration>().As<IModuleConfiguration>().Keyed<IModuleConfiguration>(key);
        }
    }
}

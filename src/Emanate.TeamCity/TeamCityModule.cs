using System.Windows.Controls;
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
        private const string Key = "teamcity";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Service.Admin.InputSelector>(Key);
            builder.RegisterType<InputSelectorViewModel>();
            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(Key);
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityMonitor>().As<IBuildMonitor>();
            builder.RegisterType<TeamCityConfiguration>().Keyed<IModuleConfiguration>(Key);
        }
    }
}

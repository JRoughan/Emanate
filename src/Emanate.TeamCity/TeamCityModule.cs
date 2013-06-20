using System.Windows.Controls;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.TeamCity.Configuration;
using Emanate.TeamCity.InputSelector;

namespace Emanate.TeamCity
{
    public class TeamCityModule : IEmanateModule
    {
        public void LoadAdminComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Service.Admin.InputSelector>("teamcity-InputSelector");
            builder.RegisterType<InputSelectorViewModel>();
            builder.RegisterType<ConfigurationView>().Keyed<UserControl>("teamcity-Config");
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityMonitor>().As<IBuildMonitor>();
            builder.RegisterType<TeamCityConfiguration>().AsSelf().As<IModuleConfiguration>().SingleInstance();
        }
    }
}

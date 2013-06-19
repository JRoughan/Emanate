using System.Windows.Controls;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.TeamCity.Configuration;
using Emanate.TeamCity.InputSelector;

namespace Emanate.TeamCity
{
    public class Module : IModule
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TeamCityConnection>().As<ITeamCityConnection>();
            builder.RegisterType<TeamCityMonitor>().As<IBuildMonitor>();
            builder.RegisterType<InputSelectorView>().Keyed<UserControl>("TeamCity-InputSelector");
            builder.RegisterType<ConfigurationView>();
            builder.RegisterType<TeamCityConfiguration>().As<IModuleConfiguration>().SingleInstance();
        }
    }
}

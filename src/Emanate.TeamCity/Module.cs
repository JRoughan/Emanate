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
            //builder.Register(c => c.Resolve<IConfigurationGenerator>().Generate<TeamCityConfiguration>()).SingleInstance();
            builder.RegisterType<TeamCityConfiguration>().SingleInstance();
            builder.RegisterType<InputSelectorView>().Keyed<UserControl>("TeamCity-InputSelector");
            builder.RegisterType<ConfigurationView>().Keyed<UserControl>("TeamCity-Configuration");
            builder.RegisterType<ConfigurationViewModel>();
        }
    }
}

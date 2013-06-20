using System.Windows.Controls;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;

namespace Emanate.Delcom
{
    public class DelcomModule : IEmanateModule
    {
        public void LoadAdminComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
            builder.RegisterType<ConfigurationView>().Keyed<UserControl>("delcom-Config");
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            builder.RegisterType<DelcomOutput>().As<IOutput>();
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>("delcom");
            builder.RegisterType<DelcomConfiguration>().AsSelf().As<IModuleConfiguration>().SingleInstance();
        }
    }
}

using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;
using Emanate.Service.Admin;

namespace Emanate.Delcom
{
    public class DelcomModule : IEmanateModule
    {
        internal const string Key = "delcom";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(Key);
            builder.RegisterType<DelcomConfigurationViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            builder.RegisterType<DelcomOutput>().As<IOutput>();
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>(Key);
            builder.RegisterType<DelcomConfiguration>().AsSelf().As<IModuleConfiguration>().SingleInstance();
        }
    }
}

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
        private const string key = "delcom";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(key);
            builder.RegisterType<DelcomConfigurationViewModel>();

            builder.RegisterType<DelcomDeviceManagerView>().Keyed<Service.Admin.DeviceManager>(key);
            builder.RegisterType<DelcomDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            RegisterCommon(builder);
            //builder.RegisterType<DelcomBuildOutput>().Keyed<IBuildOutput>(key);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>(key);
            builder.RegisterType<DelcomConfiguration>().Keyed<IModuleConfiguration>(key);
        }
    }
}

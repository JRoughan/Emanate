using System.Diagnostics;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;
using Emanate.Extensibility;

namespace Emanate.Delcom
{
    public class DelcomModule : IEmanateModule
    {
        private const string key = "delcom";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> DelcomModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(key);
            builder.RegisterType<DelcomConfigurationViewModel>();

            builder.RegisterType<DelcomDeviceManagerView>().Keyed<Extensibility.DeviceManager>(key);
            builder.RegisterType<DelcomDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> DelcomModule.LoadServiceComponents");
            RegisterCommon(builder);
            //builder.RegisterType<DelcomBuildOutput>().Keyed<IBuildOutput>(key);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> DelcomModule.RegisterCommon");
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>(key);
            builder.RegisterType<DelcomConfiguration>().As<IOutputConfiguration>().Keyed<IOutputConfiguration>(key);
        }
    }
}

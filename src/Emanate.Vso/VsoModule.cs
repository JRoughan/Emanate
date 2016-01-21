using System.Diagnostics;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Service.Admin;
using Emanate.Vso.Configuration;
using Emanate.Vso.InputSelector;

namespace Emanate.Vso
{
    public class VsoModule : IEmanateModule
    {
        private const string key = "vso";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> VsoModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Service.Admin.InputSelector>(key);
            builder.RegisterType<InputSelectorViewModel>();

            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(key);

            builder.RegisterType<VsoDeviceManagerView>().Keyed<DeviceManager>(key);
            builder.RegisterType<VsoDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> VsoModule.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<VsoMonitor>().Keyed<IBuildMonitor>(key);
        }

        private static void RegisterCommon(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> VsoModule.RegisterCommon");
            builder.RegisterType<VsoConnection>().As<IVsoConnection>();
            builder.RegisterType<VsoConfiguration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(key);
        }
    }
}

using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Extensibility;
using Emanate.Vso.Admin.Configuration;
using Emanate.Vso.Admin.Devices;
using Emanate.Vso.Admin.InputSelector;
using Serilog;
using VsoDeviceManagerView = Emanate.Vso.Admin.Devices.VsoDeviceManagerView;

namespace Emanate.Vso.Admin
{
    public class VsoAdminModule : IEmanateAdminModule, IInputModule
    {
        public string Key { get; } = "vso";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Extensibility.InputSelector>(Key);
            builder.RegisterType<InputSelectorViewModel>();

            builder.RegisterType<VsoConfigurationView>().Keyed<ConfigurationEditor>(Key);

            builder.RegisterType<VsoDeviceManagerView>().Keyed<DeviceManager>(Key);
            builder.RegisterType<VsoDeviceManagerViewModel>();
        }

        public IInputConfiguration GenerateDefaultConfig()
        {
            return new VsoConfiguration();
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.RegisterCommon");
            builder.RegisterType<VsoConnection>().As<IVsoConnection>();
            builder.RegisterType<VsoConfiguration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Extensibility;
using Emanate.Vso.Configuration;
using Emanate.Vso.InputSelector;
using Serilog;

namespace Emanate.Vso
{
    public class VsoModule : IInputModule
    {
        public string Key { get; } = "vso";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<InputSelectorView>().Keyed<Extensibility.InputSelector>(Key);
            builder.RegisterType<InputSelectorViewModel>();

            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(Key);

            builder.RegisterType<VsoDeviceManagerView>().Keyed<DeviceManager>(Key);
            builder.RegisterType<VsoDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<VsoMonitor>().Keyed<IBuildMonitor>(Key);
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

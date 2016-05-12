using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomModule : IOutputModule
    {
        public string Key { get; } = "delcom";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> DelcomModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<ConfigurationView>().Keyed<ConfigurationEditor>(Key);
            builder.RegisterType<DelcomConfigurationViewModel>();

            builder.RegisterType<DelcomDeviceManagerView>().Keyed<Extensibility.DeviceManager>(Key);
            builder.RegisterType<DelcomDeviceManagerViewModel>();
        }

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Log.Information("=> DelcomModule.LoadServiceComponents");
            RegisterCommon(builder);
            //builder.RegisterType<DelcomBuildOutput>().Keyed<IBuildOutput>(key);
        }

        public IOutputConfiguration GenerateDefaultConfig()
        {
            var config = new DelcomConfiguration();
            config.AddDefaultProfile("Default");
            return config;
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> DelcomModule.RegisterCommon");
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>(Key);
            builder.RegisterType<DelcomConfiguration>().As<IOutputConfiguration>().Keyed<IOutputConfiguration>(Key);
        }
    }
}

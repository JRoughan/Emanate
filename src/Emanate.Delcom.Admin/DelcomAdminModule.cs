﻿using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Delcom.Admin.Devices;
using Emanate.Delcom.Admin.Profiles;
using Emanate.Extensibility;
using Serilog;

namespace Emanate.Delcom.Admin
{
    public class DelcomAdminModule : IEmanateAdminModule, IOutputModule
    {
        public string Key { get; } = "delcom";

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> DelcomModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<DelcomProfileManagerView>().Keyed<ProfileManager>(Key);
            builder.RegisterType<DelcomDeviceManagerView>().Keyed<DeviceManager>(Key);
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

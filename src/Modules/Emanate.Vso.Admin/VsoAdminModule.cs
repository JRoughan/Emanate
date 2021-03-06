﻿using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Extensibility;
using Emanate.Vso.Admin.Devices;
using Emanate.Vso.Admin.Inputs;
using Emanate.Vso.Admin.Profiles;
using Serilog;

namespace Emanate.Vso.Admin
{
    public class VsoAdminModule : IEmanateAdminModule, IModule
    {
        public string Key { get; } = "vso";
        public string Name { get; } = "Visual Studio Online";
        public Direction Direction { get; } = Direction.Input;

        public void LoadAdminComponents(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.LoadAdminComponents");
            RegisterCommon(builder);
            builder.RegisterType<VsoInputSelectorView>().Keyed<InputSelector>(Key);
            builder.RegisterType<VsoInputSelectorViewModel>();

            builder.RegisterType<VsoProfileManagerView>().Keyed<ProfileManager>(Key);
            builder.RegisterType<VsoDeviceManagerView>().Keyed<DeviceManager>(Key);
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.RegisterCommon");
            builder.RegisterType<VsoConnection>().As<IVsoConnection>();
            builder.RegisterType<VsoConfiguration>().As<IInputConfiguration>().Keyed<IInputConfiguration>(Key);
        }
    }
}

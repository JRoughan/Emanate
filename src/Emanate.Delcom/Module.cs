using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Emanate.Delcom.Configuration;

namespace Emanate.Delcom
{
    public class Module : IModule
    {
        public void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DelcomOutput>().As<IOutput>();
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>("delcom");
            builder.RegisterType<ConfigurationView>();
            builder.RegisterType<DelcomConfiguration>().AsSelf().As<IModuleConfiguration>().SingleInstance();
        }
    }
}

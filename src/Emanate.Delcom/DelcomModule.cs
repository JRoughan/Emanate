using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomModule : IEmanateModule, IModule
    {
        public string Key { get; } = "delcom";
        public string Name { get; } = "Delcom";
        public Direction Direction { get; } = Direction.Output;

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Log.Information("=> DelcomModule.LoadServiceComponents");
            RegisterCommon(builder);
        }

        private void RegisterCommon(ContainerBuilder builder)
        {
            Log.Information("=> DelcomModule.RegisterCommon");
            builder.RegisterType<DelcomDevice>().Keyed<IOutputDevice>(Key);
            builder.RegisterType<DelcomConfiguration>().As<IOutputConfiguration>().Keyed<IOutputConfiguration>(Key);
        }
    }
}

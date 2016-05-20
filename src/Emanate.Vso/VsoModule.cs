using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Serilog;

namespace Emanate.Vso
{
    public class VsoModule : IEmanateModule, IModule
    {
        public string Key { get; } = "vso";
        public Direction Direction { get; } = Direction.Input;

        public void LoadServiceComponents(ContainerBuilder builder)
        {
            Log.Information("=> VsoModule.LoadServiceComponents");
            RegisterCommon(builder);
            builder.RegisterType<VsoMonitor>().Keyed<IBuildMonitor>(Key);
        }

        public IConfiguration GenerateDefaultConfig()
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

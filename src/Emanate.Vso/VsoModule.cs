using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Emanate.Vso.Configuration;
using Serilog;

namespace Emanate.Vso
{
    public class VsoModule : IEmanateModule, IInputModule
    {
        public string Key { get; } = "vso";

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

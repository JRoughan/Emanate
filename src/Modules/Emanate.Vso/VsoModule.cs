using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Input;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Emanate.Vso
{
    public class VsoModule : IEmanateModule, IModule
    {
        public string Key { get; } = "vso";
        public string Name { get; } = "Visual Studio Online";
        public Direction Direction { get; } = Direction.Input;

        public void LoadServiceComponents(IServiceCollection services)
        {
            Log.Information("=> VsoModule.LoadServiceComponents");
            RegisterCommon(services);
            services.AddSingleton<IBuildMonitorFactory, VsoMonitorFactory>();
            services.AddTransient<VsoMonitor>();
        }

        private void RegisterCommon(IServiceCollection services)
        {
            Log.Information("=> VsoModule.RegisterCommon");
            services.AddTransient<IVsoConnection, VsoConnection>();
            services.AddTransient<IInputConfiguration, VsoConfiguration>();
        }
    }
}

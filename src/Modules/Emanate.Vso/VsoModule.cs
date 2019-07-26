using Emanate.Core;
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
            services.AddTransient<IVsoConnection, VsoConnection>();
            services.AddSingleton<IBuildMonitorFactory, VsoMonitorFactory>();
            services.AddTransient<VsoMonitor>();
        }
    }
}

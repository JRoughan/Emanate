using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomModule : IEmanateModule, IModule
    {
        public string Key { get; } = "delcom";
        public string Name { get; } = "Delcom";
        public Direction Direction { get; } = Direction.Output;

        public void LoadServiceComponents(IServiceCollection services)
        {
            Log.Information("=> DelcomModule.LoadServiceComponents");
            RegisterCommon(services);
        }

        private void RegisterCommon(IServiceCollection services)
        {
            Log.Information("=> DelcomModule.RegisterCommon");
            services.AddTransient<IOutputDevice, DelcomDevice>();
            services.AddTransient<IOutputConfiguration, DelcomConfiguration>();
        }
    }
}

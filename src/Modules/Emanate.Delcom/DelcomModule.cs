using Emanate.Core;
using Emanate.Core.Output;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Emanate.Delcom
{
    public class DelcomModule : IEmanateModule
    {
        public string Key { get; } = "delcom";
        public string Name { get; } = "Delcom";
        public Direction Direction { get; } = Direction.Output;

        public void LoadServiceComponents(IServiceCollection services)
        {
            Log.Information("=> DelcomModule.LoadServiceComponents");
            services.AddTransient<IOutputDevice, DelcomDevice>();
        }
    }
}

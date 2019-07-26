using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Emanate.Core
{
    public static class ModuleLoader
    {
        public static void LoadModules(this IServiceCollection services, params IEmanateModule[] modules)
        {
            Log.Information("=> ModuleLoader.LoadModules");
            foreach (var module in modules)
            {
                Log.Information($"Loading module '{module}'");
                services.AddSingleton(module);
                module.LoadServiceComponents(services);
            }
        }
    }
}

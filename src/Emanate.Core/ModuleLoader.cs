using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Emanate.Core
{
    public static class ModuleLoader
    {
        public static void LoadModules(this IServiceCollection services)
        {
            Log.Information("=> ModuleLoader.LoadServiceModules");
            Load<IEmanateModule, EmanateModuleAttribute>(services);
        }

        private static void Load<TModule, TAttribute>(IServiceCollection services)
            where TModule : class, IEmanateModule
            where TAttribute : IModuleType
        {
            Log.Information("=> ModuleLoader.Load");
            var dlls = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Emanate.*.dll");
            foreach (var dll in dlls)
            {
                Log.Information("Scanning DLL '{0}'", Path.GetFileNameWithoutExtension(dll));
                var assembly = Assembly.LoadFrom(dll);
                var moduleAttribute = (TAttribute)assembly.GetCustomAttributes(typeof(TAttribute), false).SingleOrDefault();
                if (moduleAttribute == null)
                    continue;

                Log.Information("Loading module '{0}'", moduleAttribute.ModuleType);
                var module = Activator.CreateInstance(moduleAttribute.ModuleType) as TModule;
                if (module != null)
                {
                    services.AddSingleton<IEmanateModule>(module);
                    module.LoadServiceComponents(services);
                }
                else
                    Log.Error("Loading module '{0}'", moduleAttribute.ModuleType);
            }
        }
    }
}

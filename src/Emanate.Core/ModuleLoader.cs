using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Serilog;

namespace Emanate.Core
{
    public class ModuleLoader
    {
        public void LoadAdminModules(ContainerBuilder builder)
        {
            Log.Information("=> ModuleLoader.LoadAdminModules");
            Load<IEmanateAdminModule, EmanateAdminModuleAttribute>(builder, m => m.LoadAdminComponents(builder));
        }

        public void LoadServiceModules(ContainerBuilder builder)
        {
            Log.Information("=> ModuleLoader.LoadServiceModules");
            Load<IEmanateModule, EmanateModuleAttribute>(builder, m => m.LoadServiceComponents(builder));
        }

        private static void Load<TModule, TAttribute>(ContainerBuilder builder, Action<TModule> moduleAction)
            where TModule : class
            where TAttribute : IModuleType
        {
            Log.Information("=> ModuleLoader.Load");
            var dlls = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Emanate*.dll");
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
                    builder.RegisterInstance(module).As<IModule>();
                    moduleAction(module);
                }
                else
                    Log.Error("Loading module '{0}'", moduleAttribute.ModuleType);
            }
        }
    }
}

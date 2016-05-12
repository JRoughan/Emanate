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
            Load(builder, m => m.LoadAdminComponents(builder));
        }

        public void LoadServiceModules(ContainerBuilder builder)
        {
            Log.Information("=> ModuleLoader.LoadServiceModules");
            Load(builder, m => m.LoadServiceComponents(builder));
        }

        private static void Load(ContainerBuilder builder, Action<IEmanateModule> moduleAction)
        {
            Log.Information("=> ModuleLoader.Load");
            var dlls = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Emanate*.dll");
            foreach (var dll in dlls)
            {
                Log.Information("Scanning DLL '{0}'", Path.GetFileNameWithoutExtension(dll));
                var assembly = Assembly.LoadFrom(dll);
                var moduleAttribute = (EmanateModuleAttribute)assembly.GetCustomAttributes(typeof(EmanateModuleAttribute), false).SingleOrDefault();
                if (moduleAttribute == null)
                    continue;

                Log.Information("Loading module '{0}'", moduleAttribute.ModuleType);
                var module = Activator.CreateInstance(moduleAttribute.ModuleType) as IEmanateModule;
                if (module != null)
                {
                    if (module is IInputModule)
                        builder.RegisterInstance(module).As<IInputModule>();
                    if (module is IOutputModule)
                        builder.RegisterInstance(module).As<IOutputModule>();

                    moduleAction(module);
                }
                else
                    Log.Error("Loading module '{0}'", moduleAttribute.ModuleType);
            }
        }
    }
}

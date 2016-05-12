using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Emanate.Core
{
    public class ModuleLoader
    {
        public void LoadAdminModules(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> ModuleLoader.LoadAdminModules");
            Load(builder, m => m.LoadAdminComponents(builder));
        }

        public void LoadServiceModules(ContainerBuilder builder)
        {
            Trace.TraceInformation("=> ModuleLoader.LoadServiceModules");
            Load(builder, m => m.LoadServiceComponents(builder));
        }

        private static void Load(ContainerBuilder builder, Action<IEmanateModule> moduleAction)
        {
            Trace.TraceInformation("=> ModuleLoader.Load");
            var dlls = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Emanate*.dll");
            foreach (var dll in dlls)
            {
                Trace.TraceInformation("Scanning DLL '{0}'", Path.GetFileNameWithoutExtension(dll));
                var assembly = Assembly.LoadFrom(dll);
                var moduleAttribute = (EmanateModuleAttribute)assembly.GetCustomAttributes(typeof(EmanateModuleAttribute), false).SingleOrDefault();
                if (moduleAttribute == null)
                    continue;

                Trace.TraceInformation("Loading module '{0}'", moduleAttribute.ModuleType);
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
                    Trace.TraceError("Loading module '{0}'", moduleAttribute.ModuleType);
            }
        }
    }
}

using System;
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
            Load(m => m.LoadAdminComponents(builder));
        }

        public void LoadServiceModules(ContainerBuilder builder)
        {
            Load(m => m.LoadServiceComponents(builder));
        }

        private static void Load(Action<IEmanateModule> moduleAction)
        {
            var dlls = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Emanate*.dll");
            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFrom(dll);
                var moduleAttribute = (EmanateModuleAttribute)assembly.GetCustomAttributes(typeof(EmanateModuleAttribute), false).SingleOrDefault();
                if (moduleAttribute == null)
                    continue;

                var module = Activator.CreateInstance(moduleAttribute.ModuleType) as IEmanateModule;
                if (module != null)
                    moduleAction(module);
            }
        }
    }
}

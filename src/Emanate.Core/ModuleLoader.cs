using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;

namespace Emanate.Core
{
    public class ModuleLoader
    {
        public void LoadModules(ContainerBuilder builder)
        {
            var dlls = Directory.EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "Emanate*.dll");
            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFrom(dll);
                var moduleAttribute = (EmanateModuleAttribute)assembly.GetCustomAttributes(typeof (EmanateModuleAttribute), false).SingleOrDefault();
                if (moduleAttribute == null)
                    continue;

                var module = Activator.CreateInstance(moduleAttribute.ModuleType) as IEmanateModule;
                if (module != null)
                    module.Load(builder);
            }
        }
    }
}

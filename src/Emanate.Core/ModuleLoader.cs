using System;
using System.IO;
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
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsInterface || type.IsAbstract)
                        continue;

                    if (typeof(IModule).IsAssignableFrom(type))
                    {
                        var module = Activator.CreateInstance(type) as IModule;
                        if (module != null)
                            module.Load(builder);
                    }
                }
            }
        }
    }
}

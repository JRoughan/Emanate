using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Emanate.Core.Input;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    class PluginLoader
    {
        private readonly Type outputPluginType = typeof(IOutput);
        private readonly Type buildMonitorPluginType = typeof(IBuildMonitor);

        public Plugins Load()
        {
            var plugins = new Plugins();
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach (var file in Directory.EnumerateFiles(currentFolder, "*.dll"))
            {
                var assembly = Assembly.ReflectionOnlyLoadFrom(file);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || !type.IsPublic)
                        continue;

                    // TODO: Get interfaces once then perform both tests
                    if (type.GetInterface(outputPluginType.FullName) != null)
                    //if (outputPluginType.IsAssignableFrom(type))
                    {
                        plugins.AddOutputPlugin(type);
                    }
                    
                    // Assuming here that no plugin will fill both an input and an ouput role
                    else if (type.GetInterface(buildMonitorPluginType.FullName) != null)
                    //else if (buildMonitorPluginType.IsAssignableFrom(type))
                    {
                        plugins.AddBuildMonitorPlugin(type);
                    }
                }
            }
            return plugins;
        }
    }

    class Plugins
    {
        private readonly List<PluginType> outputPlugins = new List<PluginType>();
        private readonly List<PluginType> buildMonitorPlugins = new List<PluginType>();

        public IEnumerable<PluginType> OutputPlugins { get { return outputPlugins; } }
        public IEnumerable<PluginType> BuildMonitorPlugins { get { return buildMonitorPlugins; } }

        public void AddOutputPlugin(Type outputPluginType)
        {
            outputPlugins.Add(new PluginType(outputPluginType));
        }

        public void AddBuildMonitorPlugin(Type buildMonitorPluginType)
        {
            buildMonitorPlugins.Add(new PluginType(buildMonitorPluginType));
        }
    }
}

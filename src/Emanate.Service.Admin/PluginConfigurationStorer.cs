using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Controls;
using System.Xml.Linq;
using Autofac;
using Emanate.Core.Configuration;

namespace Emanate.Service.Admin
{
    public class PluginConfigurationStorer
    {
        private readonly IComponentContext componentContext;
        private readonly IEnumerable<IModuleConfiguration> moduleConfigurations;
        private readonly IEnumerable<IOutputDevice> outputDevices;
        private static string configFilePath;

        public PluginConfigurationStorer(IComponentContext componentContext, IEnumerable<IModuleConfiguration> moduleConfigurations, IEnumerable<IOutputDevice> outputDevices)
        {
            this.componentContext = componentContext;
            this.moduleConfigurations = moduleConfigurations;
            this.outputDevices = outputDevices;
        }

        public Foo Load()
        {
            var foo = new Foo();

            var configDoc = GetServiceConfiguration();

            var rootNode = configDoc.Element("emanate");

            // Modules
            var modules = rootNode.Element("modules");
            foreach (var moduleElement in modules.Elements())
            {
                var name = moduleElement.Name.LocalName;
                var config = moduleConfigurations.FirstOrDefault(c => c.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (config != null)
                    config.FromXml(moduleElement);
            }

            foreach (var moduleConfig in moduleConfigurations)
            {
                var gui = componentContext.Resolve(moduleConfig.GuiType) as UserControl;
                gui.DataContext = moduleConfig;
                foo.ModuleConfigurations.Add(new ConfigurationInfo(moduleConfig.Name, gui, moduleConfig));
            }

            // Output devices
            var devices = rootNode.Element("output-devices");
            foreach (var moduleElement in devices.Elements())
            {
                var name = moduleElement.Name.LocalName;
                var config = outputDevices.FirstOrDefault(c => c.Key.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (config != null)
                    config.FromXml(moduleElement);
            }

            foreach (var device in outputDevices)
            {
                foo.OutputDevices.Add(device);
            }

            return foo;
        }

        public void Save(Foo foo)
        {
            if (configFilePath == null)
                throw new InvalidOperationException("Cannot save configuration before it's been loaded");

            var configDoc = new XDocument();
            var rootElement = new XElement("emanate");
            configDoc.Add(rootElement);
            var modulesElement = new XElement("modules");
            rootElement.Add(modulesElement);

            foreach (var configurationInfo in foo.ModuleConfigurations)
            {
                var configuration = configurationInfo.ModuleConfiguration;
                var xml = configuration.ToXml();
                modulesElement.Add(xml);
            }

            configDoc.Save(configFilePath);
        }

        private static XDocument GetServiceConfiguration()
        {
            var mc = new ManagementClass("Win32_Service");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo.GetPropertyValue("Name").ToString() == "EmanateService")
                {
                    var pathToServiceExe = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    var dir = Path.GetDirectoryName(pathToServiceExe);
                    configFilePath = Path.Combine(dir, "Emanate.config");
                    return XDocument.Load(configFilePath);
                }
            }
            throw new Exception("Could not find the service or the installed path.");
        }
    }
}

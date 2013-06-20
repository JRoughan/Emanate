using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Xml.Linq;
using Autofac;
using Emanate.Core;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    public class ConfigurationCaretaker
    {
        private readonly IComponentContext componentContext;
        private readonly IEnumerable<IModuleConfiguration> moduleConfigurations;
        private static string configFilePath;

        public ConfigurationCaretaker(IComponentContext componentContext, IEnumerable<IModuleConfiguration> moduleConfigurations)
        {
            this.componentContext = componentContext;
            this.moduleConfigurations = moduleConfigurations;
        }

        public GlobalConfig Load(string configFile = null)
        {
            var foo = new GlobalConfig();

            configFilePath = configFile ?? GetServiceConfigurationFile();
            var configDoc = XDocument.Load(configFilePath);

            var rootNode = configDoc.Element("emanate");

            // Modules
            var modules = rootNode.Element("modules");
            foreach (var moduleMemento in modules.Elements("module").Select(e => new Memento(e)))
            {
                var config = moduleConfigurations.FirstOrDefault(c => c.Key.Equals(moduleMemento.Type, StringComparison.OrdinalIgnoreCase));
                if (config != null)
                    config.SetMemento(moduleMemento);
            }

            foreach (var moduleConfig in moduleConfigurations)
            {
                foo.ModuleConfigurations.Add(moduleConfig);
            }

            // Output devices
            var devices = rootNode.Element("outputs");
            foreach (var deviceMemento in devices.Elements("output").Select(e => new Memento(e)))
            {
                var device = componentContext.ResolveKeyed<IOutputDevice>(deviceMemento.Type);
                device.SetMemento(deviceMemento);
                foo.OutputDevices.Add(device);
            }

            return foo;
        }

        public void Save(GlobalConfig globalConfig)
        {
            if (configFilePath == null)
                throw new InvalidOperationException("Cannot save configuration before it's been loaded");

            var configDoc = new XDocument();
            var rootElement = new XElement("emanate");
            configDoc.Add(rootElement);


            // Modules
            var modulesElement = new XElement("modules");
            foreach (var configuration in globalConfig.ModuleConfigurations)
            {
                var moduleMemento = configuration.CreateMemento();
                modulesElement.Add(moduleMemento.Element);
            }
            rootElement.Add(modulesElement);

            // Output devices
            var devicesElement = new XElement("outputs");

            foreach (var device in globalConfig.OutputDevices)
            {
                var deviceMemento = device.CreateMemento();
                devicesElement.Add(deviceMemento.Element);
            }
            rootElement.Add(devicesElement);

            configDoc.Save(configFilePath);
        }

        private static string GetServiceConfigurationFile()
        {
            var mc = new ManagementClass("Win32_Service");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo.GetPropertyValue("Name").ToString() == "EmanateService")
                {
                    var pathToServiceExe = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    var dir = Path.GetDirectoryName(pathToServiceExe);
                    return Path.Combine(dir, "Emanate.config");
                }
            }
            throw new Exception("Could not find the service or the installed path.");
        }
    }
}

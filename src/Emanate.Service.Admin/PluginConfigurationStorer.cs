using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Controls;
using System.Xml.Linq;
using Autofac;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Service.Admin
{
    public class PluginConfigurationStorer
    {
        private readonly IComponentContext componentContext;
        private readonly IEnumerable<IModuleConfiguration> moduleConfigurations;
        private static string configFilePath;

        public PluginConfigurationStorer(IComponentContext componentContext, IEnumerable<IModuleConfiguration> moduleConfigurations)
        {
            this.componentContext = componentContext;
            this.moduleConfigurations = moduleConfigurations;
        }

        public TotalConfig Load()
        {
            var foo = new TotalConfig();

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
            foreach (var deviceElement in devices.Elements())
            {
                var name = deviceElement.Name.LocalName;
                var device = componentContext.ResolveKeyed<IOutputDevice>(name);
                device.FromXml(deviceElement);

                //foreach (var inputGroup in device.Inputs.GroupBy(i => i.Source))
                //{

                    
                //}

                var inputSelector = componentContext.ResolveKeyed<UserControl>("teamcity" + "-InputSelector");
                var outputDeviceInfo = new OutputDeviceInfo(device.Name, device, inputSelector);
                foo.OutputDevices.Add(outputDeviceInfo);
            }

            return foo;
        }

        public void Save(TotalConfig totalConfig)
        {
            if (configFilePath == null)
                throw new InvalidOperationException("Cannot save configuration before it's been loaded");

            var configDoc = new XDocument();
            var rootElement = new XElement("emanate");
            configDoc.Add(rootElement);


            // Modules
            var modulesElement = new XElement("modules");
            foreach (var configurationInfo in totalConfig.ModuleConfigurations)
            {
                var configuration = configurationInfo.ModuleConfiguration;
                var xml = configuration.ToXml();
                modulesElement.Add(xml);
            }
            rootElement.Add(modulesElement);

            // Output devices
            var devicesElement = new XElement("output-devices");

            foreach (var deviceInfo in totalConfig.OutputDevices)
            {
                var device = deviceInfo.OutputDevice;
                var xml = device.ToXml();
                devicesElement.Add(xml);
            }
            rootElement.Add(devicesElement);

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

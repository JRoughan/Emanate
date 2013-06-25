using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Autofac;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public class ConfigurationCaretaker
    {
        private static readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Emanate");
        private static readonly string configFilePath = Path.Combine(configDir, "Configuration.xml");

        private readonly IComponentContext componentContext;

        public ConfigurationCaretaker(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public GlobalConfig Load()
        {
            if (!File.Exists(configFilePath))
                return null;

            var builder = new ContainerBuilder();
            var foo = new GlobalConfig();

            var configDoc = XDocument.Load(configFilePath);
            var rootNode = configDoc.Element("emanate");

            // Modules
            var modules = rootNode.Element("modules");
            foreach (var moduleMemento in modules.Elements("module").Select(e => new Memento(e)))
            {
                var moduleConfig = componentContext.ResolveKeyed<IModuleConfiguration>(moduleMemento.Type);
                moduleConfig.SetMemento(moduleMemento);
                foo.ModuleConfigurations.Add(moduleConfig);
                builder.RegisterInstance(moduleConfig).AsSelf();
            }

            // Output devices
            var devices = rootNode.Element("outputs");
            foreach (var deviceMemento in devices.Elements("output").Select(e => new Memento(e)))
            {
                var device = componentContext.ResolveKeyed<IOutputDevice>(deviceMemento.Type);
                device.SetMemento(deviceMemento);
                foo.OutputDevices.Add(device);
            }

            builder.Update(componentContext.ComponentRegistry);

            return foo;
        }

        public void Save(GlobalConfig globalConfig)
        {
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

            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);

            configDoc.Save(configFilePath);
        }
    }
}

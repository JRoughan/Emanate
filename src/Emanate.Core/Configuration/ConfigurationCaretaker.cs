using System;
using System.Collections.Generic;
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
                return GenerateDefaultConfiguration();

            var builder = new ContainerBuilder();
            var globalConfig = new GlobalConfig();

            var configDoc = XDocument.Load(configFilePath);
            var rootNode = configDoc.Element("emanate");

            // Modules
            var modules = rootNode.Element("modules");
            foreach (var moduleMemento in modules.Elements("module").Select(e => new Memento(e)))
            {
                var moduleConfig = componentContext.ResolveKeyed<IModuleConfiguration>(moduleMemento.Type);
                moduleConfig.SetMemento(moduleMemento);
                globalConfig.ModuleConfigurations.Add(moduleConfig);
                builder.RegisterInstance(moduleConfig).AsSelf();
            }

            // Output devices
            var outputsElement = rootNode.Element("outputs");
            foreach (var outputElement in outputsElement.Elements("output"))
            {
                var outputType = outputElement.Attribute("type").Value;
                var deviceName = outputElement.Attribute("device").Value;

                var config = globalConfig.ModuleConfigurations.Single(c => c.Key == outputType);
                var device = config.OutputDevices.Single(p => p.Name == deviceName);

                var inputsElement = outputElement.Element("inputs");
                foreach (var inputElement in inputsElement.Elements("input"))
                {
                    var input = new InputInfo();
                    input.Source = inputElement.Attribute("source").Value;
                    input.Id = inputElement.Attribute("id").Value;
                    device.Inputs.Add(input);
                }

                globalConfig.OutputDevices.Add(device);
            }

            builder.Update(componentContext.ComponentRegistry);

            return globalConfig;
        }

        private GlobalConfig GenerateDefaultConfiguration()
        {
            var builder = new ContainerBuilder();

            var config = new GlobalConfig();
            foreach (var moduleConfiguration in componentContext.Resolve<IEnumerable<IModuleConfiguration>>())
            {
                builder.RegisterInstance(moduleConfiguration).AsSelf();
                config.ModuleConfigurations.Add(moduleConfiguration);
            }

            builder.Update(componentContext.ComponentRegistry);

            return config;
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
            var outputsElement = new XElement("outputs");

            foreach (var device in globalConfig.OutputDevices)
            {
                var outputElement = new XElement("output");
                outputElement.Add(new XAttribute("type", device.Key));
                outputElement.Add(new XAttribute("device", device.Name));

                var inputsElement = new XElement("inputs");
                foreach (var input in device.Inputs)
                {
                    var inputElement = new XElement("input");
                    inputElement.Add(new XAttribute("source", input.Source));
                    inputElement.Add(new XAttribute("id", input.Id));
                    inputsElement.Add(inputElement);
                }
                outputElement.Add(inputsElement);
                outputsElement.Add(outputElement);
            }
            rootElement.Add(outputsElement);

            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);

            configDoc.Save(configFilePath);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Autofac;
using Emanate.Core.Output;
using Serilog;

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

        public async Task<GlobalConfig> Load()
        {
            Log.Information("=> ConfigurationCaretaker.Load");
            return await Task.Run(() =>
            {
                if (!File.Exists(configFilePath))
                {
                    Log.Information("No config file found");
                    return GenerateDefaultConfiguration();
                }

                var builder = new ContainerBuilder();
                var globalConfig = new GlobalConfig();

                var allModules = componentContext.Resolve<IEnumerable<IModule>>();

                globalConfig.InputModules.AddRange(allModules.Where(m => m.Direction == Direction.Input));
                globalConfig.OutputModules.AddRange(allModules.Where(m => m.Direction == Direction.Output));


                Log.Information("Loading config file from '{0}'", configFilePath);
                var configDoc = XDocument.Load(configFilePath);
                var rootNode = configDoc.Element("emanate");

                if (rootNode != null)
                {
                    // Modules
                    var modules = rootNode.Element("modules");
                    if (modules != null)
                    {
                        foreach (var moduleMemento in modules.Elements("module").Select(e => new Memento(e)))
                        {
                            IOriginator moduleConfig;
                            switch (moduleMemento.Type)
                            {
                                case "output":
                                    moduleConfig = componentContext.ResolveKeyed<IOutputConfiguration>(moduleMemento.Key);
                                    globalConfig.OutputConfigurations.Add((IOutputConfiguration) moduleConfig);
                                    moduleConfig.SetMemento(moduleMemento);
                                    break;
                                case "input":
                                    moduleConfig = componentContext.ResolveKeyed<IInputConfiguration>(moduleMemento.Key);
                                    globalConfig.InputConfigurations.Add((IInputConfiguration) moduleConfig);
                                    moduleConfig.SetMemento(moduleMemento);
                                    globalConfig.InputDevices.AddRange(((IInputConfiguration)moduleConfig).Devices);
                                    break;
                                default:
                                    throw new Exception("Unknown module type");
                            }
                            builder.RegisterInstance(moduleConfig).AsSelf();
                        }
                    }
                    else
                        Log.Warning("Missing element: modules");

                    // Output devices
                    var outputsElement = rootNode.Element("outputs");
                    if (outputsElement != null)
                    {
                        foreach (var outputElement in outputsElement.Elements("output"))
                        {
                            var outputType = outputElement.GetAttributeString("type");
                            var deviceName = outputElement.GetAttributeString("device");

                            var config = globalConfig.OutputConfigurations.Single(c => c.Key == outputType);
                            var device = config.OutputDevices.Single(p => p.Name == deviceName);

                            var inputsElement = outputElement.Element("inputs");
                            if (inputsElement != null)
                            {
                                foreach (var inputElement in inputsElement.Elements("input"))
                                {
                                    var input = new InputInfo
                                    {
                                        Id = inputElement.GetAttributeString("id"),
                                        Source = inputElement.GetAttributeString("source"),
                                    };
                                    Log.Information("Adding input '{0}' to device '{1}'", input.Id, deviceName);
                                    device.Inputs.Add(input);
                                }
                            }
                            else
                                Log.Warning("Missing element: inputs");

                            Log.Information("Adding device '{0}'", deviceName);
                            globalConfig.OutputDevices.Add(device);
                        }
                    }
                    else
                        Log.Warning("Missing element: outputs");
                }
                else
                    Log.Error("Missing root node");

                Log.Information("Updating container from config");
                builder.Update(componentContext.ComponentRegistry);

                return globalConfig;
            });
        }

        private GlobalConfig GenerateDefaultConfiguration()
        {
            Log.Information("=> ConfigurationCaretaker.GenerateDefaultConfiguration");
            var builder = new ContainerBuilder();

            var config = new GlobalConfig();
            foreach (var moduleConfiguration in componentContext.Resolve<IEnumerable<IOutputConfiguration>>())
            {
                builder.RegisterInstance(moduleConfiguration).AsSelf();
                config.OutputConfigurations.Add(moduleConfiguration);
            }
            foreach (var moduleConfiguration in componentContext.Resolve<IEnumerable<IInputConfiguration>>())
            {
                builder.RegisterInstance(moduleConfiguration).AsSelf();
                config.InputConfigurations.Add(moduleConfiguration);
            }

            Log.Information("Updating container from config");
            builder.Update(componentContext.ComponentRegistry);

            return config;
        }

        public static void Save(GlobalConfig globalConfig)
        {
            Log.Information("=> ConfigurationCaretaker.Save");
            var configDoc = new XDocument();
            var rootElement = new XElement("emanate");
            configDoc.Add(rootElement);

            // Modules
            var modulesElement = new XElement("modules");
            foreach (var configuration in globalConfig.OutputConfigurations)
            {
                var moduleMemento = configuration.CreateMemento();
                modulesElement.Add(moduleMemento.Element);
            }
            foreach (var configuration in globalConfig.InputConfigurations)
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

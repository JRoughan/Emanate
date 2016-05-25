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
                                    globalConfig.OutputDevices.AddRange(((IOutputConfiguration)moduleConfig).OutputDevices);
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

                    // Mappings
                    var mappingsElement = rootNode.Element("mappings");
                    if (mappingsElement != null)
                    {
                        foreach (var mappingElement in mappingsElement.Elements("mapping"))
                        {
                            var mapping = new Mapping();
                            mapping.OutputId = mappingElement.GetAttributeGuid("output-id");

                            foreach (var inputsElements in mappingElement.Elements("inputs"))
                            {
                                var inputGroup = new InputGroup();
                                inputGroup.InputDeviceId = inputsElements.GetAttributeGuid("input-device-id");
                                foreach (var inputElement in inputsElements.Elements("input"))
                                {
                                    var inputId = inputElement.GetAttributeString("input-id");
                                    inputGroup.Inputs.Add(inputId);
                                }
                                mapping.InputGroups.Add(inputGroup);
                            }

                            Log.Information("Adding mapping for Output:'{0}' from {1}", mapping.OutputId, string.Join(", ", mapping.InputGroups.SelectMany(g => g.Inputs)));
                            globalConfig.Mappings.Add(mapping);
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

            // Mappings
            var mappingsElement = new XElement("mappings");

            foreach (var mapping in globalConfig.Mappings)
            {
                var mappingElement = new XElement("mapping");
                mappingElement.Add(new XAttribute("output-id", mapping.OutputId));

                
                foreach (var inputGroup in mapping.InputGroups)
                {
                    var inputsElement = new XElement("inputs");
                    inputsElement.Add(new XAttribute("input-device-id", inputGroup.InputDeviceId));

                    foreach (var inputId in inputGroup.Inputs)
                    {
                        var inputElement = new XElement("input");
                        inputElement.Add(new XAttribute("input-id", inputId));
                        inputsElement.Add(inputElement);
                    }

                    mappingsElement.Add(inputsElement);
                }
                mappingsElement.Add(mappingElement);
            }
            rootElement.Add(mappingsElement);

            if (!Directory.Exists(configDir))
                Directory.CreateDirectory(configDir);

            configDoc.Save(configFilePath);
        }
    }
}

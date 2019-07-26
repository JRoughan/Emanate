using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Emanate.Core.Output;
using Serilog;

namespace Emanate.Core.Configuration
{
    public class ConfigurationCaretaker
    {
        private readonly IDiskAccessor diskAccessor;
        private readonly IEnumerable<IModule> modules;

        public ConfigurationCaretaker(IDiskAccessor diskAccessor, IEnumerable<IModule> modules)
        {
            this.diskAccessor = diskAccessor;
            this.modules = modules;
        }

        public async Task<GlobalConfig> Load()
        {
            Log.Information("=> ConfigurationCaretaker.Load");
            return await Task.Run(() =>
            {
                var configDoc = diskAccessor.Load(Paths.ConfigFilePath);
                if (configDoc == null)
                {
                    Log.Information("No config file found");
                    return null;
                }

                var globalConfig = new GlobalConfig();

                globalConfig.InputModules.AddRange(modules.Where(m => m.Direction == Direction.Input));
                globalConfig.OutputModules.AddRange(modules.Where(m => m.Direction == Direction.Output));


                Log.Information("Loading config file from '{0}'", Paths.ConfigFilePath);
                var rootNode = configDoc.Element("emanate");

                if (rootNode != null)
                {
                    // Mappings
                    var mappingsElement = rootNode.Element("mappings");
                    if (mappingsElement != null)
                    {
                        foreach (var mappingElement in mappingsElement.Elements("mapping"))
                        {
                            var mapping = new Mapping();
                            mapping.OutputDeviceId = mappingElement.GetAttributeGuid("output-device-id");

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

                            Log.Information("Adding mapping for Output:'{0}' from {1}", mapping.OutputDeviceId, string.Join(", ", mapping.InputGroups.SelectMany(g => g.Inputs)));
                            globalConfig.Mappings.Add(mapping);
                        }
                    }
                    else
                        Log.Warning("Missing element: outputs");
                }
                else
                    Log.Error("Missing root node");

                return globalConfig;
            });
        }

        public void Save(GlobalConfig globalConfig)
        {
            Log.Information("=> ConfigurationCaretaker.Save");
            var configDoc = new XDocument();
            var rootElement = new XElement("emanate");
            configDoc.Add(rootElement);

            // Mappings
            var mappingsElement = new XElement("mappings");

            foreach (var mapping in globalConfig.Mappings)
            {
                var mappingElement = new XElement("mapping");
                mappingElement.Add(new XAttribute("output-device-id", mapping.OutputDeviceId));

                
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

                    mappingElement.Add(inputsElement);
                }
                mappingsElement.Add(mappingElement);
            }
            rootElement.Add(mappingsElement);

            diskAccessor.Save(configDoc, Paths.ConfigFilePath);
        }
    }

    public interface IDiskAccessor
    {
        void Save(XDocument configDoc, string path);
        XDocument Load(string path);
    }
}

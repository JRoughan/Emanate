using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Emanate.Model;
using Serilog;

namespace Emanate.Core.Configuration
{
    public class ConfigurationCaretaker
    {
        private readonly IDiskAccessor diskAccessor;

        public ConfigurationCaretaker(IDiskAccessor diskAccessor)
        {
            this.diskAccessor = diskAccessor;
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
                            var mapping = new DisplayConfiguration();
                            mapping.DisplayDeviceId = mappingElement.GetAttributeGuid("output-device-id");

                            foreach (var inputsElements in mappingElement.Elements("inputs"))
                            {
                                var inputGroup = new SourceGroup();
                                inputGroup.SourceDeviceId = inputsElements.GetAttributeGuid("input-device-id");
                                foreach (var inputElement in inputsElements.Elements("input"))
                                {
                                    var inputId = inputElement.GetAttributeString("input-id");
                                    inputGroup.SourceConfiguration.Builds += ("^^" + inputId);
                                }
                                mapping.SourceGroups.Add(inputGroup);
                            }

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
                mappingElement.Add(new XAttribute("output-device-id", mapping.DisplayDeviceId));

                
                foreach (var inputGroup in mapping.SourceGroups)
                {
                    var inputsElement = new XElement("inputs");
                    inputsElement.Add(new XAttribute("input-device-id", inputGroup.SourceDeviceId));

                    var inputElement = new XElement("input");
                    inputElement.Add(new XAttribute("input-id", inputGroup.SourceConfiguration.Builds));
                    inputsElement.Add(inputElement);

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

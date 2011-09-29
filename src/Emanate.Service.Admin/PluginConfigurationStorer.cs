using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Text.RegularExpressions;
using Emanate.Core.Configuration;

namespace Emanate.Service.Admin
{
    class PluginConfigurationStorer
    {
        public IEnumerable<ConfigurationInfo> Load()
        {
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var appConfig = GetServiceConfiguration();

            foreach (var file in Directory.EnumerateFiles(currentFolder, "*.dll"))
            {
                var assembly = Assembly.LoadFrom(file);
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface || !type.IsPublic)
                        continue;

                    var configurationAttribute = (ConfigurationAttribute)type.GetCustomAttributes(typeof(ConfigurationAttribute), false).SingleOrDefault();
                    if (configurationAttribute == null)
                        continue;

                    var properties = GetConfigProperties(type, appConfig);

                    yield return new ConfigurationInfo(configurationAttribute.Name, properties);
                }
            }
        }

        public void Save(IEnumerable<ConfigurationInfo> configurations)
        {
            var appConfig = GetServiceConfiguration();

            appConfig.AppSettings.Settings.Clear();
            foreach (var property in configurations.SelectMany(c => c.Properties))
            {
                var value = property.Value != null ? property.Value.ToString() : "";
                appConfig.AppSettings.Settings.Add(property.Key,value);
            }

            appConfig.Save(ConfigurationSaveMode.Full);
        }

        private static Configuration GetServiceConfiguration()
        {
            var mc = new ManagementClass("Win32_Service");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo.GetPropertyValue("Name").ToString() == "MonitoringService")
                {
                    var pathToServiceExe = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    return ConfigurationManager.OpenExeConfiguration(pathToServiceExe);
                }
            }
            throw new Exception("Could not find the service or the installed path.");
        }

        private IEnumerable<ConfigurationProperty> GetConfigProperties(Type configType, Configuration appConfig)
        {
            var properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in properties)
            {
                var keyAttribute = (KeyAttribute)propertyInfo.GetCustomAttributes(false).Single(a => typeof(KeyAttribute).IsAssignableFrom(a.GetType()));
                var element = appConfig.AppSettings.Settings[keyAttribute.Key];//.Value;
                var value = element != null ? element.Value : null;
                yield return new ConfigurationProperty
                                 {
                                     Name = propertyInfo.Name,
                                     FriendlyName = Regex.Replace(propertyInfo.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 "),
                                     Key = keyAttribute.Key,
                                     Type = propertyInfo.PropertyType,
                                     IsPassword = keyAttribute.IsPassword,
                                     Value = value
                                 };

            }
        }
    }
}

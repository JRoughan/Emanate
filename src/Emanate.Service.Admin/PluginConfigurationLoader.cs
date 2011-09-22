using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Emanate.Core;
using Emanate.Core.Input.TeamCity;

namespace Emanate.Service.Admin
{
    class PluginConfigurationLoader
    {
        public IEnumerable<ConfigurationInfo> Load()
        {
            var currentFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
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

                    var properties = GetConfigProperties(type);

                    yield return new ConfigurationInfo(configurationAttribute.Name, properties);
                }
            }
        }

        private IEnumerable<ConfigurationProperty> GetConfigProperties(Type configType)
        {
            var properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in properties)
            {
                var keyAttribute = (KeyAttribute)propertyInfo.GetCustomAttributes(false).Single(a => typeof(KeyAttribute).IsAssignableFrom(a.GetType()));
                yield return new ConfigurationProperty
                                 {
                                     Name = propertyInfo.Name,
                                     FriendlyName = Regex.Replace(propertyInfo.Name, "([a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))", "$1 "),
                                     Key = keyAttribute.Key,
                                     Type = propertyInfo.PropertyType
                                 };

            }
        }
    }
}

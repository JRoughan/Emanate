using System;
using System.Linq;
using System.Reflection;
using Emanate.Core.Input.TeamCity;

namespace Emanate.Core
{
    public class ReflectionConfigurationGenerator : IConfigurationGenerator
    {
        private readonly IConfigurationStorage configurationStorage;

        public ReflectionConfigurationGenerator(IConfigurationStorage configurationStorage)
        {
            this.configurationStorage = configurationStorage;
        }

        public T Generate<T>()
        {
            var configType = typeof(T);
            var config = Activator.CreateInstance<T>();

            var properties = configType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in properties)
            {
                var keyAttribute = (KeyAttribute)propertyInfo.GetCustomAttributes(false).Single(a => typeof(KeyAttribute).IsAssignableFrom(a.GetType()));
                var value = configurationStorage.GetString(keyAttribute.Key);
                propertyInfo.SetValue(config, value, null);
            }

            return config;
        }
    }
}
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
                var value = LoadValue(keyAttribute.Key, propertyInfo.PropertyType);
                propertyInfo.SetValue(config, value, null);
            }

            return config;
        }

        private object LoadValue(string key, Type propertyType)
        {
            if (propertyType == typeof(string))
                return configurationStorage.GetString(key);
            if (propertyType == typeof(int))
                return configurationStorage.GetInt(key);
            if (propertyType == typeof(bool))
                return configurationStorage.GetBool(key);

            throw new Exception("Unsupported property type");
        }
    }
}
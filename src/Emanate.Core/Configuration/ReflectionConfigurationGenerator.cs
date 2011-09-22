using System;
using System.Linq;
using System.Reflection;
using Emanate.Core.Input.TeamCity;

namespace Emanate.Core.Configuration
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
                var keyAttribute = (KeyAttribute)propertyInfo.GetCustomAttributes(false).SingleOrDefault(a => typeof(KeyAttribute).IsAssignableFrom(a.GetType()));
                if (keyAttribute == null)
                    throw new MissingKeyException(string.Format("Configuration property '{0}.{1}' is  missing required KeyAttribute", configType.Name, propertyInfo.Name));

                var value = LoadValue(keyAttribute.Key, propertyInfo);
                propertyInfo.SetValue(config, value, null);
            }

            return config;
        }

        private object LoadValue(string key, PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            if (propertyType == typeof(string))
                return configurationStorage.GetString(key);
            if (propertyType == typeof(int))
                return configurationStorage.GetInt(key);
            if (propertyType == typeof(bool))
                return configurationStorage.GetBool(key);

            throw new NotSupportedException(string.Format("Configuration property '{0}' is of unsupported type '{1}", property.Name, propertyType));
        }
    }
}
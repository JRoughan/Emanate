using System;

namespace Emanate.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class KeyAttribute : Attribute
    {
        public string Key { get; private set; }

        public KeyAttribute(string key)
        {
            Key = key;
        }

        public bool IsPassword { get; set; }
    }
}
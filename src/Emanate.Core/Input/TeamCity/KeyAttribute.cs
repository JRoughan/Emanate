using System;

namespace Emanate.Core.Input.TeamCity
{
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
        public string Key { get; private set; }

        public KeyAttribute(string key)
        {
            Key = key;
        }
    }
}
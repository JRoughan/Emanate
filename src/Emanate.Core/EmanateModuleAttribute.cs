using System;

namespace Emanate.Core
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class EmanateModuleAttribute : Attribute, IModuleType
    {
        public EmanateModuleAttribute(Type moduleType)
        {
            if (!typeof(IEmanateModule).IsAssignableFrom(moduleType))
                throw new ArgumentException("'{0}' does not implement IEmanateModule");

            ModuleType = moduleType;
        }

        public Type ModuleType { get; private set; }
    }
}

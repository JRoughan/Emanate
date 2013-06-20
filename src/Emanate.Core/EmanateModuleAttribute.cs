using System;

namespace Emanate.Core
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false, Inherited=false)]
    public class EmanateModuleAttribute : Attribute
    {
        public EmanateModuleAttribute(Type moduleType)
        {
            if (!typeof(IEmanateModule).IsAssignableFrom(moduleType))
                throw new ArgumentException("'{0}' does not implement IModule");

            ModuleType = moduleType;
        }

        public Type ModuleType { get; private set; }
    }
}

using System;

namespace Emanate.Core
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class EmanateAdminModuleAttribute : Attribute, IModuleType
    {
        public EmanateAdminModuleAttribute(Type moduleType)
        {
            if (!typeof(IEmanateAdminModule).IsAssignableFrom(moduleType))
                throw new ArgumentException("'{0}' does not implement IEmanateAdminModule");

            ModuleType = moduleType;
        }

        public Type ModuleType { get; private set; }
    }
}
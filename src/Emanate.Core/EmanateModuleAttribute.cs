using System;

namespace Emanate.Core
{
    public interface IModuleType
    {
        Type ModuleType { get; }
    }

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

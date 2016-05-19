using Autofac;
using Emanate.Core.Configuration;

namespace Emanate.Core
{
    public interface IEmanateModule
    {
        void LoadServiceComponents(ContainerBuilder builder);
    }

    public interface IEmanateAdminModule
    {
        void LoadAdminComponents(ContainerBuilder builder);
    }

    public interface IOutputModule
    {
        string Key { get; }
        IOutputConfiguration GenerateDefaultConfig();
    }

    public interface IInputModule
    {
        string Key { get; }
        IInputConfiguration GenerateDefaultConfig();
    }
}
